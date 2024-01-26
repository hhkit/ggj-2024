using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private int m_FailCount;

    public UnityEvent<Jester> OnJesterSuccess;
    public UnityEvent<Jester, RejectionReason> OnJesterFailure;
    public UnityEvent OnLevelWin;
    public UnityEvent OnLevelLose;
    public UnityEvent<Jester[]> MoveQueueForward;

    public Queue<Jester> m_JesterQueue { get; private set; }
    public King m_King { get; private set; }
    public Jester m_CurrentJester { get; private set; }

    public int m_InitialLives = 3;
    public int m_Points { get; private set; }
    public int m_Lives { get; private set; }

    public static GameSystem instance;

    private JokeManager jokeManager;
    private DayManager dayManager;
    private JesterFactory jesterFactory;

    private void Awake()
    {
        instance = this;
        jokeManager = FindObjectOfType<JokeManager>();
        dayManager = FindObjectOfType<DayManager>();
        jesterFactory = FindObjectOfType<JesterFactory>();
        Debug.Assert(jokeManager != null, "No JokeManager in scene");
        Debug.Assert(dayManager != null, "No DayManager in scene");
        Debug.Assert(jesterFactory != null, "No JesterFactory in scene");

        InitializeKing();
        PopulateJesters();
        m_Lives = m_InitialLives;

    }

    void Start()
    {
        foreach (Jester item in m_JesterQueue)
            Debug.Log(item);

        AdvanceJesterQueue();
    }



    public void ToggleJokesWindow()
    {
        UISystem.instance.ToggleJokeWindow();
    }
    private void InitializeKing()
    {
        m_King = FindObjectOfType<King>();
        Debug.Assert(m_King != null);

        var preference = dayManager.currentDay.kingPreference.Trim();
        Debug.Log($"King prefers {preference}");
        if (preference != "")
            m_King.SetJokePreference(preference);
    }

    private void PopulateJesters()
    {
        var jokeQueue = jokeManager.CreateJokeQueue(m_King, dayManager.currentDay.jesters);
        m_JesterQueue = new(jokeQueue.Select(joke =>jesterFactory.CreateJester(joke)));
    }

    public void CheckJoke()
    {
        var _jester = m_CurrentJester;
        Debug.Assert(_jester == null, "No jester in queue");

        if (m_King.ApproveJester(_jester, out RejectionReason reason))
        {
            JesterSuccess(_jester);
        }
        else
        {
            JesterFail(_jester, reason);
        }

        AdvanceJesterQueue();
    }

    private void AdvanceJesterQueue()
    {
        if (m_JesterQueue.Count == 0)
        {
            EndDay();
            m_CurrentJester = null;
        }
        m_CurrentJester = m_JesterQueue.Dequeue();
        MoveQueueForward?.Invoke(m_JesterQueue.ToArray());
    }

    void AddScore()
    {
        m_Points++;

    }
    void DeductScore()
    {

        m_Points--;
    }

    void DeductLife()
    {
        m_Lives--;
        if (m_Lives <= 0)
            Lose();
    }

    void JesterSuccess(Jester jester)
    {
        AddScore();
        OnJesterSuccess.Invoke(jester);
    }

    void JesterFail(Jester jester, RejectionReason reason)
    {
        DeductScore();
        DeductLife();
        OnJesterFailure.Invoke(jester, reason);
    }

    private void EndDay()
    {
        OnLevelWin.Invoke();
    }

    private void Lose()
    {
        OnLevelLose.Invoke();
    }

    // Player sends the Jester to the King
    public void AcceptJester()
    {
        CharacterSystem.instance.SendJesterToKing(m_CurrentJester);
        CheckJoke();
    }

    // Player refuses the Jester as audience to the King
    public void RejectJester()
    {

    }
}
