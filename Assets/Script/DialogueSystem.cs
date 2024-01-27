using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Search;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    public SpeechBubble m_SpeechBubble_Prefab;

    public List<SpeechBubble> m_Player_Jester_Convo;
    public bool m_ConvoOngoing;


    public GameObject m_SpeechBubbleHolder;

    [SerializeField] private Transform m_PlayerBubblePosition;
    [SerializeField] private Transform m_JesterBubblePosition;

    [SerializeField] private SpeechBubble m_PlayerBubble;
    [SerializeField] private SpeechBubble m_JesterBubble;
    [SerializeField] private SpeechBubble m_JesterBubble2;
    private int bubbleIndex = 0;
    private Queue<DialogAction> m_DialogQueue;

    public Action OnCurrentDialogDone;
    private JokeManager jokeManager;

    private static float TIME_TO_DISPLAY_DIALOG = 2f;

    public struct DialogAction
    {
        public DialogAction(SpeechBubble _bubble, String _text, float _timeToShow, Vector3 _position)
        {
            bubble = _bubble;
            
            timeToShow = _timeToShow;
            position = _position;

            if (_text.StartsWith(">"))
                text = _text.TrimStart('>');
            else
                text = _text;
        }
         
        public SpeechBubble bubble;
        public String text;
        public float timeToShow;
        public Vector3 position;
    }
        
    private void Awake()
    {
        instance = this;

        m_DialogQueue = new Queue<DialogAction>();
        jokeManager = FindObjectOfType<JokeManager>();
        HideDialogBox(m_PlayerBubble);
        HideDialogBox(m_JesterBubble);
        HideDialogBox(m_JesterBubble2);
        OnCurrentDialogDone += PlayNextDialog;
    }

    void Start()
    {

    }
	
    void Update()
    {
        
    }

    public void ShowDialogBox(DialogAction _action)
    {
        _action.bubble.SetText(_action.text);
        _action.bubble.gameObject.SetActive(true);
    }

    public void HideDialogBox(SpeechBubble _bubble)
    {
        _bubble.SetText(null);
        _bubble.gameObject.SetActive(false);
    }

    public void StartJokeDialog(Jester _jester)
    {
        if (m_ConvoOngoing)
            return;
        List<QuoteData> tmp = new List<QuoteData>();
        foreach (var item in jokeManager.jokeData.PlayerLines)
        {
            if (item.Context.Equals("SendToKing"))
                tmp.Add(item);
        }
        m_ConvoOngoing = true;
        var randPunchLine = tmp[UnityEngine.Random.Range(0, tmp.Count - 1)];

        m_DialogQueue.Enqueue(new DialogAction(m_PlayerBubble, randPunchLine.Lines[0], TIME_TO_DISPLAY_DIALOG, m_PlayerBubblePosition.position));

        foreach (var item in _jester.m_Joke.Lines)
        {
            m_DialogQueue.Enqueue(new DialogAction(GetConvoSpeechBubble(item), item, TIME_TO_DISPLAY_DIALOG, GetConvoSpeechPosition(item)));
        }

        PlayNextDialog();
    }

    public SpeechBubble GetConvoSpeechBubble(String _text)
    {
        if (_text.StartsWith(">"))
            return m_PlayerBubble;
        else
        {
            if (bubbleIndex++ == 0)
            {
                return m_JesterBubble;
            }
            else
            {
                bubbleIndex = 0;
                return m_JesterBubble2;
            }
        }
    }

    public Vector3 GetConvoSpeechPosition(String _text)
    {
        if (_text.StartsWith(">"))
            return m_PlayerBubblePosition.position;
        
        return m_JesterBubblePosition.position;
    }

    public void PlayNextDialog()
    {
        if (m_DialogQueue.Count == 0)
        {
        }
        else
        {
            m_ConvoOngoing = false;
            var tmp = m_DialogQueue.Dequeue();
            StartCoroutine(DialogPlaying(tmp));
        }
    }

    IEnumerator DialogPlaying(DialogAction _action)
    {
        float timer = _action.timeToShow;
        ShowDialogBox(_action);
        if (_action.bubble.m_IsPlayerBubble)
            _action.bubble.SetPosition(m_PlayerBubblePosition.position);
        else
            _action.bubble.SetPosition(m_JesterBubblePosition.position);

        bool hasStartFade = false;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            if (!hasStartFade && timer < 0.5f)
            {
                hasStartFade = true;
                _action.bubble.MoveToPosition(_action.bubble.transform.position + Vector3.up, 0.5f);
                _action.bubble.FadeAway(0.5f);
            }
        }
        _action.bubble.ResetAlpha();

        HideDialogBox(_action.bubble);

        PlayNextDialog();
    }

}
