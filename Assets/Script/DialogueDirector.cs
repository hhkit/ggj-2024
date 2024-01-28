using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Search;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;



public class DialogueDirector : MonoBehaviour
{
    public static DialogueDirector instance;

    public List<SpeechBubble> m_Player_Jester_Convo;


    private JokeManager jokeManager;
    private DialogueManager dialogManager;

    public static float TIME_TO_DISPLAY_DIALOG = 2f;

        
    private void Awake()
    {
        instance = this;
        jokeManager = FindObjectOfType<JokeManager>();
        dialogManager = FindObjectOfType<DialogueManager>();
    }

    private IEnumerator SubmitAndWaitForDialog(SpeakerId who, string line, float dur)
    {
        bool isDone = false;
        dialogManager.PushDialog(
                who,
                line,
                dur,
                () => isDone = true);
        while (isDone == false)
            yield return 0;
    }

    public void StartJokeDialog(Jester _jester, Action OnShowLast = null)
    {
        var invitationLine = jokeManager.jokeData.PlayerLines.GetRandomWhere(line => line.Context == "PunchlinesPlease");
        dialogManager.PushDialog(SpeakerId.Player, string.Join(" ", invitationLine.Lines), TIME_TO_DISPLAY_DIALOG);

        int count = 0;
        foreach (var item in _jester.m_Joke.Lines)
        {
            ++count;
            var isFinal = count == _jester.m_Joke.Lines.Length;

            var id = IsPlayerDialog(item) ? SpeakerId.Player : SpeakerId.Jester;
            dialogManager.PushDialog(id, item, TIME_TO_DISPLAY_DIALOG, isFinal ? OnShowLast : null);
        }
    }

    public IEnumerator PlaySendToKingDialog()
    {
        return SubmitAndWaitForDialog(SpeakerId.Player,
                jokeManager.jokeData.PlayerLines.GetRandomWhere(qd => qd.Context == "SendToKing").Lines[0],
                TIME_TO_DISPLAY_DIALOG);
    }

    public IEnumerator PlayRejectDialog()
    {
        yield return SubmitAndWaitForDialog(
                SpeakerId.Player,
                jokeManager.jokeData.PlayerLines.GetRandomWhere(qd => qd.Context == "DenyAudience").Lines[0],
                TIME_TO_DISPLAY_DIALOG);

        yield return SubmitAndWaitForDialog(
                SpeakerId.Jester,
                jokeManager.jokeData.JesterLines.GetRandomWhere(qd => qd.Context == "Rejection").Lines[0],
                TIME_TO_DISPLAY_DIALOG);
    }

    private bool IsPlayerDialog(string text)
    {
        return text.StartsWith(">");
    }

    public IEnumerator PlayKingDialog(bool _jokefunny, RejectionReason reason)
    {
        string context;
        if (_jokefunny)
            context = "Approve";
        else
        {
            switch (reason) {
                default:
                case RejectionReason.None:
                case RejectionReason.NotFunny:
                case RejectionReason.NotPreferred:
                    context = "RejectUnfunny";
                    break;
                case RejectionReason.Repeat:
                    context = "RejectRepeat";
                    break;
            }
        }

        var line = jokeManager.jokeData.KingLines.GetRandomWhere(qd => qd.Context == context);

        yield return SubmitAndWaitForDialog(SpeakerId.King, line.Lines[0], TIME_TO_DISPLAY_DIALOG);
    }
}
