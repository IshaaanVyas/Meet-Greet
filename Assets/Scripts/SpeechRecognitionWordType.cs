using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;



[Serializable]
public class SpeechRecognitionWordType {
    public float conf;
	public float end;
	public float start;
    public string word;
}