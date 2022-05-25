using System;
using UnityEngine;
using TMPro;

public class GeneratedObject: MonoBehaviour
{
    public TMPro.TextMeshPro m_TextTarget;

    public void UpdateProps(String text) { 
        if (m_TextTarget != null) {
            m_TextTarget.text = text;
	    }
    }
}