using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    public List<SpeechBubble> m_Player_Jester_Convo;

    private JokeManager jokeManager;
    private DialogueManager dialogManager;
        
    private void Awake()
    {
        instance = this;
        jokeManager = FindObjectOfType<JokeManager>();
        dialogManager = FindObjectOfType<DialogueManager>();
    }

    public void StartJokeDialog(Jester _jester)
    {
        var invitationLine = jokeManager.jokeData.PlayerLines.GetRandomWhere(line => line.Context == "PunchlinesPlease");
        dialogManager.PushDialog(SpeakerId.Player, string.Join(" ", invitationLine.Lines));

        foreach (var item in _jester.m_Joke.Lines)
        {
            var id = IsPlayerDialog(item) ? SpeakerId.Player : SpeakerId.Jester;
            dialogManager.PushDialog(id, item);
        }
    }

    private bool IsPlayerDialog(string text)
    {
        return text.StartsWith(">");
    }

    public void PlayKingDialog(bool _jokefunny, RejectionReason reason)
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

        dialogManager.PushDialog(SpeakerId.King, line.Lines[0]);
    }
}
