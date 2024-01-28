using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class DialogueDirector : MonoBehaviour
{
    public static DialogueDirector instance;

    public List<SpeechBubble> m_Player_Jester_Convo;

    private JokeManager jokeManager;
    private DialogueManager dialogManager;
        
    private void Awake()
    {
        instance = this;
        jokeManager = FindObjectOfType<JokeManager>();
        dialogManager = FindObjectOfType<DialogueManager>();
    }

    private IEnumerator SubmitAndWaitForDialog(SpeakerId who, string line)
    {
        bool isDone = false;
        dialogManager.PushDialog(
                who,
                line,
                false,
                () => isDone = true);
        while (isDone == false)
            yield return 0;
    }

    public void StartJokeDialog(Jester _jester, Action OnShowLast = null)
    {
        var invitationLine = jokeManager.jokeData.PlayerLines.GetRandomWhere(line => line.Context == "PunchlinesPlease");
        dialogManager.PushDialog(SpeakerId.Player, string.Join(" ", invitationLine.Lines), false);

        int count = 0;
        foreach (var item in _jester.m_Joke.Lines)
        {
            ++count;
            var isFinal = count == _jester.m_Joke.Lines.Length;

            var id = IsPlayerDialog(item) ? SpeakerId.Player : SpeakerId.Jester;
            dialogManager.PushDialog(id, item, isFinal, isFinal ? OnShowLast : null);
        }
    }

    public IEnumerator PlaySendToKingDialog()
    {
        return SubmitAndWaitForDialog(SpeakerId.Player,
                jokeManager.jokeData.PlayerLines.GetRandomWhere(qd => qd.Context == "SendToKing").Lines[0]);
    }

    public IEnumerator PlayRejectDialog()
    {
        yield return SubmitAndWaitForDialog(
                SpeakerId.Player,
                jokeManager.jokeData.PlayerLines.GetRandomWhere(qd => qd.Context == "DenyAudience").Lines[0]);

        yield return SubmitAndWaitForDialog(
                SpeakerId.Jester,
                jokeManager.jokeData.JesterLines.GetRandomWhere(qd => qd.Context == "Rejection").Lines[0]);
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

        yield return SubmitAndWaitForDialog(SpeakerId.King, line.Lines[0]);
    }
}
