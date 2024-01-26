using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RejectionReason
{
    None,
    NotFunny,
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

    public bool PrefersJoke(Joke joke)
    {
        return m_EnablePreferenceCheck && joke.Tags.Contains(m_JokePreference) == false;
    }

    public void SetJokePreference(string tag)
    {
        m_JokePreference = tag;
        m_EnablePreferenceCheck = tag != "";
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

        if (PrefersJoke(_jester.m_Joke))
        {
            Debug.Log("Joker failed");
            rejectReason = RejectionReason.NotPreferred;
            return false;
        }

        if (_jester.m_Joke.IsLame)
        {
            Debug.Log("Joker failed");
            rejectReason = RejectionReason.NotFunny;
            return false;
        }

        Debug.Log("Joker successful");
        rejectReason = RejectionReason.None;
        return true;
    }
}
