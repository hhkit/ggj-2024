using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : MonoBehaviour
{
    public string m_JokePreference;
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

    public bool ApproveJester(Jester _jester)
    {
        if (_jester == null)
            return false;

        if (_jester.m_Joke.Tags.Contains(m_JokePreference))
        {
            Debug.Log("Joker successful");
            return true;
        }

        Debug.Log("Joker failed");
        return false;
    }
}
