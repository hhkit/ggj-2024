using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private int m_FailCount;

    public UnityEvent<Jester> OnJesterSuccess;
    public UnityEvent<Jester> OnJesterFailure;
    public UnityEvent OnLevelWin;
    public UnityEvent OnLevelLose;

    public List<Jester> m_InitialList;
    public Queue<Jester> m_JesterQueue;
    public King m_King;
    public Jester m_CurrentJester;
    public int m_Points;
    public int m_Lives = 3;


    public static GameSystem instance;

    private JokeManager jokeManager;

    void Start()
    {
        m_JesterQueue = new Queue<Jester>();
        foreach (Jester item in m_InitialList)
            m_JesterQueue.Enqueue(item);

        m_InitialList.Clear();
        m_Lives = 3;

        jokeManager = FindObjectOfType<JokeManager>();
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
        // todo
    }

    public void CheckJoke()
    {
        var _jester = m_CurrentJester;
        if (_jester == null)
        {
            Debug.LogError("No jester in queue");
            return;
        }

        if (m_King.ApproveJester(_jester))
        {
            JesterSuccess(_jester);
        }
        else
        {
            JesterFail(_jester);
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

    void JesterFail(Jester jester)
    {
        DeductScore();
        DeductLife();
        OnJesterFailure.Invoke(jester);
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
