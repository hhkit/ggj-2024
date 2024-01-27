using DG.Tweening;
#if UNITY_EDITOR
using EasyButtons;
#endif
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private int m_FailCount;
    [SerializeField] private bool m_StartJesterConvoOnArrival;
    [SerializeField] private bool m_WaitingForPlayerChoice;

    public UnityEvent<Jester> OnJesterSuccess;
    public UnityEvent<Jester, RejectionReason> OnJesterFailure;

    public Queue<Jester> m_JesterQueue { get; private set; } = new();
    public King m_King { get; private set; }
    public Jester m_CurrentJester { get; private set; }

    public int m_Submitted { get; private set; }
    public int m_Points { get; private set; }
    public int m_Quota { get; private set; }

    public static GameSystem instance;

    private JokeManager jokeManager;
    private DayManager dayManager;
    private JesterFactory jesterFactory;
    private EndDayController endDayController;

    void Awake()
    {
        instance = this;
        jokeManager = FindObjectOfType<JokeManager>();
        dayManager = FindObjectOfType<DayManager>();
        jesterFactory = FindObjectOfType<JesterFactory>();
        Debug.Assert(jokeManager != null, "No JokeManager in scene");
        Debug.Assert(dayManager != null, "No DayManager in scene");
        Debug.Assert(jesterFactory != null, "No JesterFactory in scene");

        endDayController = FindObjectOfType<EndDayController>(true);

        InitializeKing();
        PopulateJesters();
        m_Points = 0;
        m_Submitted = 0;
        m_Quota = dayManager.currentDay.quota;
        m_StartJesterConvoOnArrival = true;
    }

    void Start()
    {
        foreach (Jester item in m_JesterQueue)
            Debug.Log(item);

        AdvanceJesterQueue()
            .OnComplete(StartJesterConversation);
    }

    public void ToggleJokesWindow()
    {
        UISystem.instance.ToggleJokeWindow();
    }

    void InitializeKing()
    {
        m_King = FindObjectOfType<King>();
        Debug.Assert(m_King != null);

        var preference = dayManager.currentDay.kingPreference.Trim();
        Debug.Log($"King prefers {preference} today");
        if (preference != "")
            m_King.SetJokePreference(preference);
    }

    void PopulateJesters()
    {
        var jokeQueue = jokeManager.CreateJokeQueue(m_King, dayManager.currentDay.jesters);
        m_JesterQueue = new(jokeQueue.Select(jesterFactory.CreateJester));
        Debug.Log($"created {m_JesterQueue.Count()} jesters from {jokeQueue.Count()} jokes");
    }

    void CheckJoke(Jester jester)
    {
        m_Submitted += 1;
        Debug.Assert(jester != null, "Invalid jester");
        if (m_King.ApproveJester(jester, out RejectionReason reason))
            JesterSuccess(jester);
        else
            JesterFail(jester, reason);
    }

    Tween AdvanceJesterQueue()
    {
        if (m_JesterQueue.Count == 0)
        {
            m_CurrentJester = null;
            return null;
        }

        // TODO: potential atom bomb waiting to go off
        if (m_CurrentJester != null)
            m_JesterQueue.Dequeue();

        m_CurrentJester = m_JesterQueue.FirstOrDefault();
        return WaypointManager.instance.MoveJesters(new(m_JesterQueue));
    }

    void AddScore()
    {
        m_Points++;
    }
    void DeductScore()
    {

        m_Points--;
    }

    void JesterSuccess(Jester jester)
    {
        AddScore();
        WaypointManager.instance.PlayKingAcceptJester(jester);
        OnJesterSuccess.Invoke(jester);
    }

    void JesterFail(Jester jester, RejectionReason reason)
    {
        DeductScore();
        OnJesterFailure.Invoke(jester, reason);
    }

    // This begins the end of day sequence
    Tween EndDay()
    {
        var winFlag = m_Points >= m_Quota;
        return endDayController.PlayEndingSequence(m_Submitted, m_Quota, winFlag);
    }

#if UNITY_EDITOR
    [Button]
#endif
    // Player sends the Jester at the front of the line to the King
    public Tween AcceptJester()
    {
        var jester = m_CurrentJester;
        var jesterMoveTween = 
            WaypointManager.instance.SendJesterToKing(jester)
                .OnComplete(() => CheckJoke(jester));

        var queueMoveTween = AdvanceJesterQueue();
        m_StartJesterConvoOnArrival = true;
        return DOTween.Sequence()
            .Join(jesterMoveTween)
            .Join(queueMoveTween)
            .OnComplete(StartJesterConversation);
    }

#if UNITY_EDITOR
    [Button]
#endif

    // Player refuses the Jester as audience to the King
    public Tween RejectJester()
    {
        var jester = m_CurrentJester;
        var jesterMoveTween = WaypointManager.instance.RefuseJester(jester)
            .OnKill(() => Destroy(jester.gameObject));
        var queueMoveTween = AdvanceJesterQueue();
        m_StartJesterConvoOnArrival = true;
        return queueMoveTween
            .OnComplete(StartJesterConversation);
    }

    public void OnAcceptClick()
    {
        if (m_WaitingForPlayerChoice)
        {
            AcceptJester();
            m_WaitingForPlayerChoice = false;
        }
    }

    public void OnDeclineClick()
    {
        if (m_WaitingForPlayerChoice)
        {
            RejectJester();
            m_WaitingForPlayerChoice = false;
        }
    }

    public void StartJesterConversation()
    {
        if (m_JesterQueue.Count() == 0)
        {
            EndDay();
            return;
        }

        if (m_StartJesterConvoOnArrival)
        {
            m_StartJesterConvoOnArrival = false;
            DialogueSystem.instance.StartJokeDialog(m_CurrentJester);
        }
    }

    public void ReplayJesterConversation()
    {
        DialogueSystem.instance.StartJokeDialog(m_CurrentJester);
    }

    public void WaitingForPlayerChoice()
    {
        m_WaitingForPlayerChoice = true;
    }
}
