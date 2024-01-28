using UnityEngine;
using System;
using System.Linq;

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
        // Debug.Assert(corrects.Count() > 0, "no correct jokes?");
        var unrelatedJokes = funnyJokes.Where(joke => !king.PrefersJoke(joke));

        // todo: weight the incorrect choices
        var unrelatedCount = UnityEngine.Random.Range(0, Math.Min(config.lame, unrelatedJokes.Count()));
        var unfunnyCount = UnityEngine.Random.Range(0, Math.Min(config.lame - unrelatedCount, unfunnyJokes.Count()));
        var repeatCount = Math.Min(config.lame - unrelatedCount - unfunnyCount, corrects.Count());

        var unrelated = unrelatedJokes.OrderBy(a => Guid.NewGuid()).Take(unrelatedCount);
        var unfunnies = unfunnyJokes.OrderBy(a => Guid.NewGuid()).Take(unfunnyCount);
        var repeats = corrects.OrderBy(a => Guid.NewGuid()).OrderBy(a => Guid.NewGuid()).Take(repeatCount);

        var incorrects = unrelated.Concat(unfunnies).Concat(repeats);

        var results = corrects.Concat(incorrects);

        return results.OrderBy(a => Guid.NewGuid()).ToArray();
    }
}