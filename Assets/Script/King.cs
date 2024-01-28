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
        return m_EnablePreferenceCheck == false || joke.Tags.Contains(m_JokePreference) == false;
    }

    public void SetJokePreference(string tag)
    {
        m_JokePreference = tag;
        m_EnablePreferenceCheck = tag != "";
    }

    public bool ApproveJester(Jester _jester, out RejectionReason rejectReason)
    {
        Debug.Assert(_jester != null);

        string[] kingDenySounds = { "KingDenySound1", "KingDenySound2" };

        if (m_PreviousJokes.Contains(_jester.m_Joke))
        {
            AudioManager.PlayOneShot(kingDenySounds[Random.Range(0, 2)]);
            Debug.Log("Joker failed: heard it before");
            rejectReason = RejectionReason.Repeat;
            return false;
        }

        if (PrefersJoke(_jester.m_Joke) == false)
        {
            AudioManager.PlayOneShot(kingDenySounds[Random.Range(0, 2)]);
            Debug.Log("Joker failed: not preferred joke");
            rejectReason = RejectionReason.NotPreferred;
            return false;
        }

        if (_jester.m_Joke.IsLame)
        {
            AudioManager.PlayOneShot(kingDenySounds[Random.Range(0, 2)]);
            Debug.Log("Joker failed: joke is not funny");
            rejectReason = RejectionReason.NotFunny;
            return false;
        }

        AudioManager.PlayOneShot("KingApproveSound");
        AudioManager.PlayOneShot("CoinShower");
        Debug.Log("Joker successful");
        rejectReason = RejectionReason.None;
        m_PreviousJokes.Add(_jester.m_Joke);

        return true;
    }
}
