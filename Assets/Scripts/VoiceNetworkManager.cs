using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using NativeWebSocket;

using TMPro;

/// <summary>
/// Class to manage WS connection with the ASR server. 
/// </summary>
public class VoiceNetworkManager : MonoBehaviour
{
	public TMP_Text RecognitionResultTMP;
	public Material head2;
	public GameObject testText;
	public ObjectManager m_ObjectManager;
	private static int count = 0;
	private Vector3 myVector = new Vector3(0.009f, 0.198f, 0.8f);

	private static VoiceNetworkManager _instance;

	public static VoiceNetworkManager Instance
	{
		get { return _instance; }
	}

	//public InstructionMappingPanel InstructionQuad;

	private string LastInstructionTagID = "";
	private bool vComm = false;
	private bool typing = false;
	private GameObject latestOne = null;

	private Hashtable HotwordsMap = new Hashtable() {
		// 		{ "190", AudioSourceBlueBlock } , // "81" },
		// { "23" , AudioSourceDoubleYellowBlock }, // "38" },
		// { "187", AudioSourceBigBlock }, // "77" },
		// { "16", AudioSourceSingleYellowBlock }, // "44" },
		// { "71", AudioSourceRedBlock }, // "11" },
		// { "165", AudioSourceOrangeBlock } // "69" }
		//{ "pink block", "187" },
		//{ "blue block", "190" },
		//{ "double yellow", "23" },
		//{ "single yellow", "16" },
		//{ "red block", "71" },
		//{ "read block", "71" },	
		//{ "orange block", "165" }
		{"start", "369" },
		{"and", "370" }
	};

	WebSocket websocket;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	// Start is called before the first frame update
	async void Start()
	{
		// Test JSON Decode. 
		// var test = JsonConvert.DeserializeObject<List<float>>("[0, 0]");
		// var test = JsonUtility.FromJson<DetectedObjectTypeList>("{\"objs\": [{\"markerid\": 1, \"coords\": [0, 0]}]}");
		// Debug.Log("decoded == ");
		// // var values = test.ToArray();
		// Debug.Log(test.objs[0].markerid);

		// StartCoroutine(GetRequest("http://192.168.86.248:3000/ping"));

		websocket = new WebSocket(GlobalConstants.ASRSERVER_IP);
		websocket.OnOpen += () =>
		{
			Debug.Log("Connection open!");

			// _recognizerReady = true;

				// Debug.Log("Recognizer ready");


		};

		websocket.OnError += (e) =>
		{
			Debug.Log("Error! " + e);
		};

		websocket.OnClose += (e) =>
		{
			Debug.Log("Connection closed!");
		};

		websocket.OnMessage += (bytes) =>
		{
			//Debug.Log("OnMessage!");

			var message = System.Text.Encoding.UTF8.GetString(bytes);
			//Debug.Log(message);

			var result = JsonUtility.FromJson<SpeechPartialResultType>(message);
			if (result != null) {
				// Debug.Log("======= Got partial == " + result.partial);
				if (result.partial != null && result.partial.Length > 0) {
					RecognitionResultTMP.text = result.partial;
					

					//// Search for substring query. 
					foreach (DictionaryEntry hotwordpair in HotwordsMap) 
					{
						if (result.partial.IndexOf((string)hotwordpair.Key, 0) >= 0) {
					//		// Only trigger diff., 
							if (!LastInstructionTagID.Equals((string)hotwordpair.Value)) {
								if (hotwordpair.Value.Equals("369") && vComm == false)
                                {
									head2.SetColor("_Color", Color.green);
									vComm = true;
									typing = true;

								} else if (hotwordpair.Value.Equals("370") && vComm == true)
                                {
									head2.SetColor("_Color", Color.blue);
									vComm = false;
									//GameObject newText = Instantiate(testText, testText.transform.position, Quaternion.identity);
									//m_ObjectManager.AddInteractiveObject(count.ToString(), newText);
									//count++;
									//if (count > 5)
                                    //{
									//	m_ObjectManager.DeleteAllObjects();
                                    //}

								}
								//InstructionQuad.VoiceTriggerInstruction(true, (string)hotwordpair.Value);

								LastInstructionTagID = (string)hotwordpair.Value;
							}
						}
					}
					if (vComm == true)
					{
						testText.GetComponentInChildren<TextMesh>().text = result.partial;
						if (typing == true)
						{
							typing = false;
							GameObject newText = Instantiate(testText, myVector, Quaternion.identity); //testText.transform.position
							latestOne = newText;
							m_ObjectManager.AddInteractiveObject(count.ToString(), newText);
							count++;
							if (count > 5)
							{
								m_ObjectManager.DeleteAllObjects();
							}
						} else
                        {
							latestOne.GetComponentInChildren<TextMesh>().text = result.partial;
                        }

					}
				}
			}
			// // Debug.Log("decoded == ");
			// // var values = test.ToArray();
			// // Debug.Log(test.objs[0].markerid);

			// if (test.objs.Count > 0)
			// {
			// 	foreach (DetectedObjectType detected in test.objs)
			// 	{
			// 		ScreenOutProjector.Instance.SetMarkerPixelPos(
			// 			detected.markerid.ToString(),
			// 			(detected.coords[0] + detected.coords[4]) / 2.0f,
			// 			(detected.coords[1] + detected.coords[5]) / 2.0f
			// 		);
			// 	}
			// 	// Set largest marker as the instruction set. 
			// 	ScreenOutProjector.Instance.SetInstruction(test.objs[0].markerid.ToString());
			// }

			// getting the message as a string
			// var message = System.Text.Encoding.UTF8.GetString(bytes);
			// Debug.Log("OnMessage! " + message);
		};

		// waiting for messages
		await websocket.Connect();
	}

	// Update is called once per frame
	void Update()
	{
		websocket.DispatchMessageQueue();
	}

	async public void SendConfig(string sampleRate) 
	{
			// Send Sample Rate Config.
		await websocket.SendText("{ \"config\": { \"sample_rate\" : " + sampleRate + " } }");
	}

	async public void SendVoiceData(byte[] data)
	{
		await websocket.Send(data);
	}

	async void SendWebSocketMessage()
	{
		if (websocket.State == WebSocketState.Open)
		{
			// Sending bytes
			await websocket.Send(new byte[] { 10, 20, 30 });

			// Sending plain text
			await websocket.SendText("plain text message");
		}
	}

	private async void OnApplicationQuit()
	{
		await websocket.Close();
	}

}
