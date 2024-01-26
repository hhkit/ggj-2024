

using System;
using System.Linq;
using UnityEngine;

public class JokeManager : MonoBehaviour
{
    public JokesDataSO jokeData;
    public GameObject jesterPrefab;

    private Joke[] funnyJokes;
    private Joke[] unfunnyJokes;

    public void Awake()
    {
        funnyJokes = jokeData.Jokes.Where(a => a.IsFunny).ToArray();
        unfunnyJokes = jokeData.Jokes.Where(a => a.IsLame).ToArray();
    }

    public Joke[] CreateJokeQueue(JesterConfig config)
    {
        // wtf why is my brain working not
        return new Joke[0];
    }
}