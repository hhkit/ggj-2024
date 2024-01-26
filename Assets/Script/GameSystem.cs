using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private int m_FailCount;

    public List<Jester> m_InitialList;
    public Queue<Jester> m_JesterQueue;
    public King m_King;
    public Jester m_CurrentJester;
    public int m_Points;
    public int m_Lives = 3;


    public static GameSystem instance;
    void Start()
    {
        m_JesterQueue = new Queue<Jester>();
        foreach (Jester item in m_InitialList)
            m_JesterQueue.Enqueue(item);

        m_InitialList.Clear();

        foreach (Jester item in m_JesterQueue.ToArray())
            Debug.Log(item);

        m_CurrentJester = IncrementQueue();
        m_Lives = 3;
    }
	
    void Update()
    {
        
    }


    public void AddScore()
    {

    }
    public void DeductScore()
    {

    }

    public void DeductLife()
    {

    }

    public void AcceptCurrentJoker()
    {
        KingInteract(m_CurrentJester);
    }

    public void DeclineCurrentJoker()
    {
        m_CurrentJester = IncrementQueue();
    }

    public void ToggleJokesWindow()
    {
        UISystem.instance.ToggleJokeWindow();
    }
    public void SelectJoke()
    {

    }

    public void KingInteract(Jester _jester)
    {
        if (_jester == null)
        {
            Debug.Log("No jester");
            return;
        }

        if (m_King.ApproveJester(_jester))
        {
            JesterSuccess();
        }
        else
        {
            JesterFail();
        }

        m_CurrentJester = IncrementQueue();

    }
    public Jester IncrementQueue()
    {
        if (m_JesterQueue.Count == 0)
        {
            EndDay();
            return null;
        }
        return m_JesterQueue.Dequeue();
    }

    public void JesterSuccess()
    {
        m_Points++;
    }

    public void JesterFail()
    {
        m_Points--;
        m_Lives--;
        if (m_Lives <= 0)
            Lose();

    }

    public void EndDay()
    {

    }

    public void Lose()
    {

    }
}
