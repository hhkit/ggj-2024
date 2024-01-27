using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    public SpeechBubble m_SpeechBubble_Prefab;

    public List<SpeechBubble> m_Player_Jester_Convo;
    private int _float;

    public GameObject m_SpeechBubbleHolder;

    public SpeechBubble m_PlayerBubble;
    public SpeechBubble m_JesterBubble;

    private void Awake()
    {
        instance = this;
        HideDialogBox(m_PlayerBubble);
        HideDialogBox(m_JesterBubble);
    }

    void Start()
    {

    }
	
    void Update()
    {
        
    }

    public void ShowDialogBox(SpeechBubble _bubble, string _text)
    {
        _bubble.SetText(_text);
        _bubble.gameObject.SetActive(true);
    }

    public void HideDialogBox(SpeechBubble _bubble)
    {
        _bubble.SetText(null);
        _bubble.gameObject.SetActive(false);
    }

    public void StartJokeDialog(Jester _jester, GameObject _player)
    {
        SpeechBubble playerBubble = m_Player_Jester_Convo[0];
        SpeechBubble jesterBubble = m_Player_Jester_Convo[1];

        float convoDelay = 1.5f;
        float currentConvoDelay = 0;

        string sampleText01 = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.";
        string sampleText02 = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.";
        string sampleText03 = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.";
        string sampleText04 = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.";

        currentConvoDelay += convoDelay;
        StartCoroutine(PlayDialog(playerBubble, sampleText01, currentConvoDelay)); 
        currentConvoDelay += convoDelay;
        StartCoroutine(PlayDialog(jesterBubble, sampleText02, currentConvoDelay));

    }

    IEnumerator PlayDialog(SpeechBubble _bubble,string _text, float _time)
    {
        while (_time > 0)
        {
            _time -= Time.deltaTime;
            yield return null;

        }

        ShowDialogBox(_bubble, _text);
    }
}
