using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Search;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;



public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

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

    public void StartJokeDialog(Jester _jester)
    {
        var invitationLine = jokeManager.jokeData.PlayerLines.GetRandomWhere(line => line.Context == "PunchlinesPlease");
        dialogManager.PushDialog(SpeakerId.Player, string.Join(" ", invitationLine.Lines), TIME_TO_DISPLAY_DIALOG);

        foreach (var item in _jester.m_Joke.Lines)
        {
            var id = IsPlayerDialog(item) ? SpeakerId.Player : SpeakerId.Jester;
            dialogManager.PushDialog(id, item, TIME_TO_DISPLAY_DIALOG);
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

        dialogManager.PushDialog(SpeakerId.King, line.Lines[0], TIME_TO_DISPLAY_DIALOG);
    }
}
