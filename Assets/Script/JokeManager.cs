

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

        // todo: weight the incorrect choices
        var unrelatedCount = UnityEngine.Random.Range(0, config.lame);
        var unfunnyCount = UnityEngine.Random.Range(0, config.lame - unrelatedCount);
        var repeatCount = UnityEngine.Random.Range(0, config.lame - unrelatedCount - unfunnyCount);

        var unrelated = funnyJokes.Where(joke => !king.PrefersJoke(joke)).OrderBy(a => Guid.NewGuid()).Take(unrelatedCount);
        var unfunnies = unfunnyJokes.OrderBy(a => Guid.NewGuid()).Take(unfunnyCount);
        var repeats = corrects.OrderBy(a => Guid.NewGuid()).OrderBy(a => Guid.NewGuid()).Take(repeatCount);

        var incorrects = unrelated.Concat(unfunnies).Concat(repeats);

        return corrects.Concat(incorrects).OrderBy(a => Guid.NewGuid()).ToArray();
    }
}