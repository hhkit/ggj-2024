

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

    public Joke[] CreateJokeQueue(King king, JesterConfig config)
    {
        // OrderBy(a=>Guid.NewGuid) randomizes the list
        var corrects = funnyJokes.Where(joke => king.PrefersJoke(joke)).OrderBy(a => Guid.NewGuid()).Take(config.funny);

        var unrelated = funnyJokes.Where(joke => !king.PrefersJoke(joke));
        var unfunnies = unfunnyJokes;
        var repeats = corrects.OrderBy(a => Guid.NewGuid());

        var incorrects = unrelated.Concat(unfunnies).Concat(repeats).Take(config.lame);

        return corrects.Concat(incorrects).OrderBy(a => Guid.NewGuid()).ToArray();
    }
}