using System.Collections;
using System.Collections.Generic;
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

    public Queue<Jester> m_JesterQueue;
    public King m_King;
    public Jester m_CurrentJester;
    public int m_Points;
    public int m_Lives = 3;

    public static GameSystem instance;

    private JokeManager jokeManager;
    private DayManager dayManager;
    private JesterFactory jesterFactory;

    void Start()
    {
        PopulateJesters();

        m_Lives = 3;

        jokeManager = FindObjectOfType<JokeManager>();
        dayManager = FindObjectOfType<DayManager>();
        jesterFactory = FindObjectOfType<JesterFactory>();

        foreach (Jester item in m_JesterQueue)
            Debug.Log(item);

        AdvanceJesterQueue();
    }

    void Update()
    {

    }




    public void ToggleJokesWindow()
    {
        UISystem.instance.ToggleJokeWindow();
    }
    public void PopulateJesters()
    {
        var jokeQueue = jokeManager?.CreateJokeQueue(dayManager.currentDay.jesters);

        foreach (var joke in jokeQueue)
        {
            // create jesters, set jokes
            var jester = jesterFactory.CreateJester(joke);
            Debug.Assert(jester, "Jester Prefab should contain Jester component");
            m_JesterQueue.Enqueue(jester);
        }
    }

    public void CheckJoke()
    {
        var _jester = m_CurrentJester;
        if (_jester == null)
        {
            Debug.LogError("No jester in queue");
            return;
        }

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
}
