using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class JesterFactory : MonoBehaviour
{
    public GameObject jesterPrefab;

    public Jester CreateJester(Joke joke)
    {
        var jester = Instantiate(jesterPrefab).GetComponent<Jester>();
        jester.m_Joke = joke;
        return jester;
    }
}

