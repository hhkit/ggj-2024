using DG.Tweening;
#if UNITY_EDITOR
using EasyButtons;
#endif
using System;
using System.Linq;
using UnityEngine;

public class EndDayController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Prelude;
    public TMPro.TextMeshProUGUI Title;
    public TMPro.TextMeshProUGUI JesterCountDisplay;
    public TMPro.TextMeshProUGUI JokeCountDisplay;
    public TMPro.TextMeshProUGUI MoodYesDisplay;
    public TMPro.TextMeshProUGUI MoodNoDisplay;
    public TMPro.TextMeshProUGUI SurvivalYesDisplay;
    public TMPro.TextMeshProUGUI SurvivalNoDisplay;

    public float PauseInSeconds;

    private TMPro.TextMeshProUGUI[] tmps;

    void Start()
    {
        tmps = new TMPro.TextMeshProUGUI[] {
                Prelude,
                JesterCountDisplay,
                JokeCountDisplay,
                MoodYesDisplay,
                MoodNoDisplay,
                SurvivalYesDisplay,
                SurvivalNoDisplay,
            };

        Reset();
    }
    void Reset()
    {
        foreach (var tmp in tmps)
            tmp.enabled = false;
    }

#if UNITY_EDITOR
    [Button]
#endif
    Tween PlayEndingSequence(int jesterCount, int jokeSuccess, bool success)
    {
        Debug.Log("nani");


        var titles = Title.text.Split('\n');
        Title.text = "";

        foreach (var line in titles)
            Debug.Log("nani" + line);

        JesterCountDisplay.text = $"{jesterCount}";
        JokeCountDisplay.text = $"{jokeSuccess}";


        var count = 0;
        Action AdvanceTitle = () =>
        {
            var prev = Title.text;
            Debug.Log($"count: {count} -> {Title.text}");
            while (Title.text.Trim() == prev.Trim() && titles.Count() >= count)
            {
                count += 1;
                Title.text = string.Join("\n", titles.Take(count));
            }
        };

        var seq = DOTween.Sequence();
        seq.AppendCallback(() => Prelude.enabled = true)
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle())
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => JesterCountDisplay.enabled = true)
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle())
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => JokeCountDisplay.enabled = true)
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle())
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() =>
                (success ? MoodYesDisplay : MoodNoDisplay).enabled = true
                ).AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle());

        if (success)
        {
            seq.AppendInterval(PauseInSeconds * 3);
            seq.AppendCallback(() => SurvivalYesDisplay.enabled = true);
        }
        else
        {
            // drop player char
            //seq.AppendCallback(() => SurvivalYesDisplay.enabled = true);
            seq.AppendInterval(PauseInSeconds);
            seq.AppendCallback(() => SurvivalYesDisplay.enabled = true);
        }

        return seq;
    }
}
