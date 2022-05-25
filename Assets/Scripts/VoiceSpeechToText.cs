using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

using NativeWebSocket;

public class VoiceSpeechToText : MonoBehaviour
{

	public VoiceProcessor VoiceProcessor;

	/// <summary>
	/// Server connection to voice network. Split from this due to thread incompatibility..
	/// </summary>
	public VoiceNetworkManager VoiceNetworkManager;


	[Tooltip("The Max number of alternatives that will be processed.")]
	public int MaxAlternatives = 3;

	[Tooltip("How long should we record before restarting?")]
	public float MaxRecordLength = 5;

	[Tooltip("Should the recognizer start when the application is launched?")]
	public bool AutoStart = true;

	[Tooltip("The phrases that will be detected. If left empty, all words will be detected.")]
	public List<string> KeyPhrases = new List<string>();

	//Holds all of the audio data until the user stops talking.
	private readonly List<short> _buffer = new List<short>();


	//Called when the the state of the controller changes.
	public Action<string> OnStatusUpdated;

	//Called after the user is done speaking and vosk processes the audio.
	public Action<string> OnTranscriptionResult;

	//Conditional flag to see if a recognizer has already been created.
	//TODO: Allow for runtime changes to the recognizer.
	private bool _recognizerReady;


	private WebSocket ASRWebSocket;

	//A string that contains the keywords in Json Array format
	private string _grammar = "";

	//Flag that is used to wait for the the script to start successfully.
	private bool _isInitializing;

	//Flag that is used to check if Vosk was started.
	private bool _didInit;

	//Threading Logic

	// Flag to signal we are ending
	private bool _running;

	//Thread safe queue of microphone data.
	private readonly ConcurrentQueue<short[]> _threadedBufferQueue = new ConcurrentQueue<short[]>();

	//Thread safe queue of resuts
	private readonly ConcurrentQueue<string> _threadedResultQueue = new ConcurrentQueue<string>();


	// static readonly ProfilerMarker voskRecognizerCreateMarker = new ProfilerMarker("VoskRecognizer.Create");
	// static readonly ProfilerMarker voskRecognizerReadMarker = new ProfilerMarker("VoskRecognizer.AcceptWaveform");

	//If Auto start is enabled, starts vosk speech to text.
	void Start()
	{
		if (AutoStart)
		{
			StartVoskStt();
		}
	}

	/// <summary>
	/// Start Vosk Speech to text
	/// </summary>
	/// <param name="keyPhrases">A list of keywords/phrases. Keywords need to exist in the models dictionary, so some words like "webview" are better detected as two more common words "web view".</param>
	/// <param name="modelPath">The path to the model folder relative to StreamingAssets. If the path has a .zip ending, it will be decompressed into the application data persistent folder.</param>
	/// <param name="startMicrophone">"Should the microphone after vosk initializes?</param>
	/// <param name="maxAlternatives">The maximum number of alternative phrases detected</param>
	public void StartVoskStt(List<string> keyPhrases = null, string modelPath = default, bool startMicrophone = false, int maxAlternatives = 3)
	{
		if (_isInitializing)
		{
			Debug.LogError("Initializing in progress!");
			return;
		}
		if (_didInit)
		{
			Debug.LogError("Vosk has already been initialized!");
			return;
		}

		if (keyPhrases != null)
		{
			KeyPhrases = keyPhrases;
		}

		MaxAlternatives = maxAlternatives;
		StartCoroutine(DoStartVoskStt(startMicrophone));
	}

	//Decompress model, load settings, start Vosk and optionally start the microphone
	private IEnumerator DoStartVoskStt(bool startMicrophone)
	{
		_isInitializing = true;
		yield return WaitForMicrophoneInput();

		// OnStatusUpdated?.Invoke("Loading Model from: " + _decompressedModelPath);
		//Vosk.Vosk.SetLogLevel(0);
		// _model = new Model(_decompressedModelPath);

		yield return null;

		OnStatusUpdated?.Invoke("Initialized");
		VoiceProcessor.OnFrameCaptured += VoiceProcessorOnOnFrameCaptured;
		VoiceProcessor.OnRecordingStop += VoiceProcessorOnOnRecordingStop;

		if (startMicrophone)
			VoiceProcessor.StartRecording();

		_isInitializing = false;
		_didInit = true;

		ToggleRecording();
	}

	//Translates the KeyPhraseses into a json array and appends the `[unk]` keyword at the end to tell vosk to filter other phrases.
	// private void UpdateGrammar()
	// {
	// 	if (KeyPhrases.Count == 0)
	// 	{
	// 		_grammar = "";
	// 		return;
	// 	}

	// 	JSONArray keywords = new JSONArray();
	// 	foreach (string keyphrase in KeyPhrases)
	// 	{
	// 		keywords.Add(new JSONString(keyphrase.ToLower()));
	// 	}

	// 	keywords.Add(new JSONString("[unk]"));

	// 	_grammar = keywords.ToString();
	// }

	//Wait until microphones are initialized
	private IEnumerator WaitForMicrophoneInput()
	{
		while (Microphone.devices.Length <= 0)
			yield return null;
	}

	//Can be called from a script or a GUI button to start detection.
	public void ToggleRecording()
	{
		Debug.Log("Toogle Recording");
		if (!VoiceProcessor.IsRecording)
		{
			Debug.Log("Start Recording");
			_running = true;
			VoiceProcessor.StartRecording();
			Task.Run(ThreadedWork); // .ConfigureAwait(false);
		}
		else
		{
			Debug.Log("Stop Recording");
			_running = false;
			VoiceProcessor.StopRecording();
		}
	}

	//Calls the On Phrase Recognized event on the Unity Thread
	void Update()
	{
		if (ASRWebSocket != null) 
		{
    		ASRWebSocket.DispatchMessageQueue();
		}
		if (_threadedResultQueue.TryDequeue(out string voiceResult))
		{
			OnTranscriptionResult?.Invoke(voiceResult);
		}
	}

	//Callback from the voice processor when new audio is detected
	private void VoiceProcessorOnOnFrameCaptured(short[] samples)
	{
		_threadedBufferQueue.Enqueue(samples);
	}

	//Callback from the voice processor when recording stops
	private void VoiceProcessorOnOnRecordingStop()
	{
		Debug.Log("Stopped");
	}

	//Feeds the autio logic into the vosk recorgnizer
	private async Task ThreadedWork()
	{
		// voskRecognizerCreateMarker.Begin();
		if (!_recognizerReady)
		{
			VoiceNetworkManager.SendConfig(VoiceProcessor.SampleRate.ToString());
			_recognizerReady = true;
		}

		// voskRecognizerCreateMarker.End();

		// voskRecognizerReadMarker.Begin();

		while (_running)
		{
			if (_threadedBufferQueue.TryDequeue(out short[] voiceResult))
			{
				// Debug.Log("Send Voice Len: " + voiceResult.Length);
				byte[] data = new byte[voiceResult.Length * 2]; 
  				Buffer.BlockCopy(voiceResult, 0, data, 0, voiceResult.Length * 2);

				VoiceNetworkManager.SendVoiceData(data);
				// if (_recognizer.AcceptWaveform(voiceResult, voiceResult.Length))
				// {
				// 	var result = _recognizer.Result();
				// 	_threadedResultQueue.Enqueue(result);
				// }
			}
			else
			{
				// Wait for some data
				await Task.Delay(100);
			}
		}

		// voskRecognizerReadMarker.End();
	}

}

