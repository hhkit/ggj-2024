

using System;
using System.Linq;
using UnityEngine;

public class JokeManager : Manager
{
    public JokesDataSO jokeData;

    private Joke[] funnyJokes;
    private Joke[] unfunnyJokes;

    public override void ManagerInit()
    {
        funnyJokes = jokeData.Jokes.Where(a => a.IsFunny).ToArray();
        unfunnyJokes = jokeData.Jokes.Where(a => a.IsLame).ToArray();
    }

    public Joke[] CreateJokeQueue(King king, JesterConfig config)
    {
        // OrderBy(a=>Guid.NewGuid) randomizes the list
        var corrects = funnyJokes.Where(joke => king.PrefersJoke(joke)).OrderBy(a => Guid.NewGuid()).Take(config.funny);

        // todo: weight the incorrect choices
        var unrelatedCount = UnityEngine.Random.Range(0, config.lame);
        var unrelated = funnyJokes.Where(joke => !king.PrefersJoke(joke));
        var unfunnies = unfunnyJokes;
        var repeats = corrects.OrderBy(a => Guid.NewGuid());

        var incorrects = unrelated.Concat(unfunnies).Concat(repeats).OrderBy(a => Guid.NewGuid()).Take(config.lame);

        return corrects.Concat(incorrects).OrderBy(a => Guid.NewGuid()).ToArray();
    }
}