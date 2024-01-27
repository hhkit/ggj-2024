using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    public string m_Text;
    [SerializeField] private TextMeshPro m_TextUI;

    void Start()
    {
        
    }
	
    void Update()
    {

    }

    public void SetText(string _text)
    {
        if (_text == null)
            return;
        m_TextUI.SetText(_text);
    }

}
