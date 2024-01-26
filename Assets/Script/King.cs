using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{
    public JokeType m_JokePreference;
    void Start()
    {
        
    }
	
    void Update()
    {
        
    }

    public bool ApproveJester(Jester _jester)
    {
        if (_jester == null)
            return false;

        if (_jester.m_Joke.jokeType == m_JokePreference)
        {
            Debug.Log("Joker successful");
            return true;
        }

        Debug.Log("Joker failed");
        return false;
    }
}
