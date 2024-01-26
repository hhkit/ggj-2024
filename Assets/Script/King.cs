using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RejectionReason
{
    None,
    Repeat,
    NotPreferred,
}

public class King : MonoBehaviour
{
    public string m_JokePreference;
    public bool m_EnablePreferenceCheck = false;
    private HashSet<Joke> m_PreviousJokes = new(); 

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetJokePreference(string tag)
    {
        m_JokePreference = tag;
    }

    public bool ApproveJester(Jester _jester, out RejectionReason rejectReason)
    {
        Debug.Assert(_jester != null);

        if (m_PreviousJokes.Contains(_jester.m_Joke))
        {
            Debug.Log("Joker failed");
            rejectReason = RejectionReason.Repeat;
            return false;
        }

        if (m_EnablePreferenceCheck && _jester.m_Joke.Tags.Contains(m_JokePreference) == false)
        {
            Debug.Log("Joker failed");
            rejectReason = RejectionReason.NotPreferred;
            return false;
        }

        Debug.Log("Joker successful");
        rejectReason = RejectionReason.None;
        return true;
    }
}
