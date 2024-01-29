using DG.Tweening;
#if UNITY_EDITOR
using EasyButtons;
#endif
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum WinState
{
    Win,
    Lose,
    Death,
}

public class EndDayController : MonoBehaviour
{

    public TMPro.TextMeshProUGUI Prelude;
    public TMPro.TextMeshProUGUI Title;
    public TMPro.TextMeshProUGUI JesterCountDisplay;
    public TMPro.TextMeshProUGUI JokeCountDisplay;
    public TMPro.TextMeshProUGUI MoodYesDisplay;
    public TMPro.TextMeshProUGUI MoodNoDisplay;
    public TMPro.TextMeshProUGUI MoodDeathDisplay;
    public TMPro.TextMeshProUGUI SurvivalYesDisplay;
    public TMPro.TextMeshProUGUI SurvivalNoDisplay;
    public TMPro.TextMeshProUGUI SurvivalDeathDisplay;

    public Button NextDayButton;
    public Camera EndCamConfig;

    public float PauseInSeconds;

    private TMPro.TextMeshProUGUI[] tmps;
    private bool successFlag;
    private DayManager dayManager;
    [SerializeField] private SpriteRenderer trapDoorRenderer;
    [SerializeField] private Sprite trapDoorOpen;
    [SerializeField] private GameObject player;

    void Start()
    {
        dayManager = FindObjectOfType<DayManager>();

        tmps = new TMPro.TextMeshProUGUI[] {
                Prelude,
                JesterCountDisplay,
                JokeCountDisplay,
                MoodYesDisplay,
                MoodNoDisplay,
                MoodDeathDisplay,
                SurvivalYesDisplay,
                SurvivalNoDisplay,
                SurvivalDeathDisplay,
            };

        Reset();
    }
    void Reset()
    {
        foreach (var tmp in tmps)
            tmp.enabled = false;
        NextDayButton.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    [Button]
#endif
    public Tween PlayEndingSequence(int jesterCount, int jokeSuccess, WinState success)
    {
        var gameUI = FindObjectOfType<GameUIController>();
        gameUI.gameObject.SetActive(false);

        AudioManager.PlayBGM("Tax-Office-Night-PM-Music", 0.2f);
        successFlag = success == WinState.Win;

        TMPro.TextMeshProUGUI MoodDisplay = null;
        TMPro.TextMeshProUGUI SurvivalDisplay = null;
        switch (success)
        {
            case WinState.Win:
                MoodDisplay = MoodYesDisplay;
                SurvivalDisplay = SurvivalYesDisplay;
                break;
            case WinState.Lose:
                MoodDisplay = MoodNoDisplay;
                SurvivalDisplay = SurvivalNoDisplay;
                break;
            case WinState.Death:
                MoodDisplay = MoodDeathDisplay;
                SurvivalDisplay = SurvivalDeathDisplay;
                Prelude.text = "The king has come to an end.";
                break;
        }

        NextDayButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
            successFlag ? "Next Day" : "Try Again";
        
        var titles = Title.text.Split('\n');
        Title.text = "";

        foreach (var line in titles)
            Debug.Log("nani" + line);

        JesterCountDisplay.text = $"{jesterCount}";
        JokeCountDisplay.text = $"{jokeSuccess}";

        var camTween = DOTween.Sequence();
        {
            float camDur = 1.0f;
            camTween.Join(Camera.main.transform.DOMove(EndCamConfig.transform.position, camDur));
            camTween.Join(Camera.main.DOOrthoSize(EndCamConfig.orthographicSize, camDur));
        };
    
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
            AudioManager.PlayOneShot("DayEndWriting");
        };

        var seq = DOTween.Sequence();

        seq.Append(camTween)
            .AppendInterval(PauseInSeconds);
        seq.AppendCallback(() => gameObject.SetActive(true))
            .AppendInterval(PauseInSeconds);
        seq.AppendCallback(() =>
                {
                    Prelude.enabled = true;
                    AudioManager.PlayOneShot("stamp");
                })
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle())
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => {
                    JesterCountDisplay.enabled = true;
                    AudioManager.PlayOneShot("stamp");
                })
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle())
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => {
            JokeCountDisplay.enabled = true;
                AudioManager.PlayOneShot("stamp");
            })
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle())
                .AppendInterval(PauseInSeconds)
            .AppendCallback(() =>
                {
                    MoodDisplay.enabled = true;
                    AudioManager.PlayOneShot("stamp");
                }
                ).AppendInterval(PauseInSeconds)
            .AppendCallback(() => AdvanceTitle());

        if (successFlag)
        {
            seq.AppendInterval(PauseInSeconds * 3);
            seq.AppendCallback(() => { SurvivalYesDisplay.enabled = true; AudioManager.PlayOneShot("stamp"); });
        }
        else
        {
            // drop player char
            seq.AppendInterval(PauseInSeconds * 3);
            seq.Append(player.transform.DORotate(new Vector3(0, 0, -30), 0.2f)
                    .OnStart(() =>
                    {
                        trapDoorRenderer.sprite = trapDoorOpen;
                        AudioManager.PlayOneShot("Trapdoor");
                    }))
                    .Join(player.transform.DOScale(0.02f, 0.8f))
                    .Join(player.transform.DOMoveY(player.transform.position.y - 0.15f, 0.8f))
                .Append(player.transform.DOScale(0.01f,0.1f));

            seq.AppendInterval(PauseInSeconds);
            seq.AppendCallback(() => { SurvivalDisplay.enabled = true; AudioManager.PlayOneShot("stamp"); });
        }
        seq.AppendInterval(PauseInSeconds);
        seq.AppendCallback(() => NextDayButton.gameObject.SetActive(true));

        return seq;
    }

    public void LoadNextScene()
    {
        var currScene = SceneManager.GetActiveScene();
        var fade = FindObjectOfType<FadeOut>();
        var img = fade.GetComponent<Image>();
        if (successFlag)
        {
            DayManager.CurrentLevel += 1;
        }

        img.DOFade(1.0f, 0.5f).OnComplete(() =>
        {
            if (DayManager.CurrentLevel < dayManager.days.Length)
            {
                SceneManager.LoadScene(currScene.buildIndex);
            }
            else
            {
                DayManager.CurrentLevel -= 1;
                SceneManager.LoadScene("BadEnd");
            }
        });
    }
}
