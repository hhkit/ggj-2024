using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Search;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public enum SpeechBubbleId
{
    Player,
    Jester,
    King,
}

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
    [SerializeField] private SpeechBubble m_Kingbubble;
    private Queue<DialogAction> m_DialogQueue;

    private JokeManager jokeManager;

    private static float TIME_TO_DISPLAY_DIALOG = 2f;

    public struct DialogAction
    {
        public DialogAction(SpeechBubbleId _id, String _text, float _timeToShow)
        {
            id = _id;
            
            timeToShow = _timeToShow;

            if (_text.StartsWith(">"))
                text = _text.TrimStart('>');
            else
                text = _text;
            myIndex = bubbleIndex++ % 2;
        }
         
        public SpeechBubbleId id;
        public String text;
        public float timeToShow;

        public int myIndex { get; private set; }

        private static int bubbleIndex;
    }
        
    private void Awake()
    {
        instance = this;

        m_DialogQueue = new Queue<DialogAction>();
        jokeManager = FindObjectOfType<JokeManager>();
        HideDialogBox(m_PlayerBubble);
        HideDialogBox(m_JesterBubble);
        HideDialogBox(m_JesterBubble2);
    }

    public void ShowDialogBox(DialogAction _action)
    {
        var bubble = GetConvoSpeechBubble(_action);
        bubble.SetText(_action.text);
        bubble.gameObject.SetActive(true);
    }

    public void HideDialogBox(SpeechBubble _bubble)
    {
        _bubble.SetText(null);
        _bubble.gameObject.SetActive(false);
    }
    public void PushDialog(SpeechBubbleId id, string line, float dur)
    {
        m_DialogQueue.Enqueue(new DialogAction(id, line, dur));
    }

    public void StartJokeDialog(Jester _jester)
    {
        if (m_ConvoOngoing)
            return;

        var invitationLine = jokeManager.jokeData.PlayerLines.GetRandomWhere(line => line.Context == "PunchlinesPlease");
        m_ConvoOngoing = true;

        PushDialog(SpeechBubbleId.Player, string.Join(" ", invitationLine.Lines), TIME_TO_DISPLAY_DIALOG);

        foreach (var item in _jester.m_Joke.Lines)
        {
            var id = IsPlayerDialog(item) ? SpeechBubbleId.Player : SpeechBubbleId.Jester; 
            PushDialog(id, item, TIME_TO_DISPLAY_DIALOG);
        }

        PlayNextDialog(_jester);
    }


    private bool IsPlayerDialog(string text)
    {
        return text.StartsWith(">");
    }

    private SpeechBubble GetConvoSpeechBubble(DialogAction action)
    {
        switch (action.id)
        {
            case SpeechBubbleId.Player:
                return m_PlayerBubble;
            case SpeechBubbleId.Jester:
                return action.myIndex == 0 ? m_JesterBubble : m_JesterBubble2;
            case SpeechBubbleId.King:
                return m_Kingbubble;
        }
        Debug.Assert(false, "unreachable code");
        return null;
    }

    private Vector3 GetConvoSpeechPosition(SpeechBubbleId id)
    {
        switch (id)
        {
            case SpeechBubbleId.Player: 
                return m_PlayerBubblePosition.position;
            case SpeechBubbleId.Jester:
                return m_JesterBubblePosition.position;
            case SpeechBubbleId.King:
                return Vector3.zero;
        }
        Debug.Assert(false, "unreachable code");
        return Vector3.zero;
    }

    public void PlayNextDialog(Jester _jester)
    {
        if (m_DialogQueue.Count == 0)
        {
            m_ConvoOngoing = false;
            GameSystem.instance.WaitingForPlayerChoice();
        }
        else
        {
            var tmp = m_DialogQueue.Dequeue();
            StartCoroutine(DialogPlaying(tmp, _jester));
        }
    }

    IEnumerator DialogPlaying(DialogAction _action, Jester _jester)
    {
        float timer = _action.timeToShow;
        ShowDialogBox(_action);

        var bubble = GetConvoSpeechBubble(_action);
        var position = GetConvoSpeechPosition(_action.id);

        bubble.SetPosition(position);

        bool hasStartFade = false;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            if (!hasStartFade && timer < 0.5f)
            {
                hasStartFade = true;
                bubble.MoveToPosition(bubble.transform.position + Vector3.up, 0.5f);
                bubble.FadeAway(0.5f);
            }
        }
        bubble.ResetAlpha();

        HideDialogBox(bubble);

        PlayNextDialog(_jester);
    }

    public void PlayKingDialog(bool _jokefunny, RejectionReason reason)
    {
        string context;
        if (_jokefunny)
            context = "Approve";
        else
        {
            switch (reason) {
                default:
                case RejectionReason.None:
                case RejectionReason.NotFunny:
                case RejectionReason.NotPreferred:
                    context = "RejectUnfunny";
                    break;
                case RejectionReason.Repeat:
                    context = "RejectRepeat";
                    break;
            }
        }

        var line = jokeManager.jokeData.KingLines.GetRandomWhere(qd => qd.Context == context);

        StartCoroutine(KingDialogPlaying(new DialogAction(SpeechBubbleId.King, line.Lines[0], TIME_TO_DISPLAY_DIALOG)));
    }

    IEnumerator KingDialogPlaying(DialogAction _action)
    {
        var bubble = GetConvoSpeechBubble(_action);
        float timer = _action.timeToShow;
        ShowDialogBox(_action);

        bool hasStartFade = false;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            if (!hasStartFade && timer < 0.5f)
            {
                hasStartFade = true;
                bubble.FadeAway(0.5f);
            }
        }
        bubble.ResetAlpha();

        HideDialogBox(bubble);

    }

}
