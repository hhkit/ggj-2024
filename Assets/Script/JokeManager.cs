

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
        var repeatCount = UnityEngine.Random.Range(0, config.lame);
        var unfunnyCount = config.lame - repeatCount;

        // OrderBy(a=>Guid.NewGuid) randomizes the list
        var funnies = funnyJokes.OrderBy(a => Guid.NewGuid()).Take(config.funny);
        var repeats = funnies.OrderBy(a => Guid.NewGuid()).Take(repeatCount);
        var unfunnies = unfunnyJokes.OrderBy(a => Guid.NewGuid()).Take(unfunnyCount);

        var unorderedQueue = funnies.Concat(repeats).Concat(unfunnies);
        return unorderedQueue.OrderBy(a => Guid.NewGuid()).ToArray();
    }
}