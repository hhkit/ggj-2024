using DG.Tweening;
using EasyButtons;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public enum SpeakerId
{
    Player,
    Jester,
    King,
}
public class DialogueManager : Manager
{
    public Transform m_PlayerBubblePosition;
    public Transform m_JesterBubblePosition;
    public SpeechBubble m_PlayerBubble;
    public SpeechBubble m_JesterBubble;
    public SpeechBubble m_KingBubble;


    public float entryDur = 1f;
    public float exitDur = 2f;
    public float entryOffset = 0.5f;
    public float exitOffset = 1f;
    private struct DialogAction
    {
        public DialogAction(SpeakerId _id, string _text, Action _exitCallback)
        {
            id = _id;
            text = _text.TrimStart('>');
            exitCallback = _exitCallback;
        }

        public SpeakerId id;
        public string text;
        public Action exitCallback;
    }

    public bool isRunning { get => isShowing; }

    Tween currentBubble = null;
    Queue<DialogAction> m_DialogQueue = new();
    bool isShowing = false;

    public override void ManagerInit()
    {
        // we will instantiate bubbles instead
        m_PlayerBubble.gameObject.SetActive(false);
        m_JesterBubble.gameObject.SetActive(false);
        m_KingBubble.gameObject.SetActive(false);
    }

    [Button]
    public void Test(
        SpeakerId who,
        string text)
    {
        PushDialog(who, text);
    }
    public void PushDialog(
        SpeakerId who, 
        string text,
        Action exitCallback = null
    )
    {
        m_DialogQueue.Enqueue(new DialogAction(
            who, text, exitCallback));
    }

    private bool ShowNextDialog()
    {
        if (m_DialogQueue.Count == 0)
            return false;

        if (isShowing)
            return false;

        var top = m_DialogQueue.Dequeue();

        SpeechBubble bubblePrefab = null;
        {
            switch (top.id)
            {
                case SpeakerId.Player:
                    bubblePrefab = m_PlayerBubble;
                    break;
                case SpeakerId.King:
                    bubblePrefab = m_KingBubble;
                    break;
                case SpeakerId.Jester:
                    bubblePrefab = m_JesterBubble;
                    break;
            }

            Debug.Assert(bubblePrefab != null);
        }

        var newBubble = Instantiate(bubblePrefab);
        newBubble.gameObject.SetActive(true);
        newBubble.SetText(top.text);

        var seq = newBubble.Play(entryDur, exitDur, entryOffset, exitOffset);
        if (top.exitCallback != null)
            seq.AppendCallback(() => top.exitCallback());
        seq.AppendCallback(() => isShowing = false);
        isShowing = true;

        return true;
    }

    void Update()
    {
        /*
         * speech bubble will have:
         *  entry
         *  show
         *  exit
         *  
         * dialogue will entry, show, then overlap exit and entry
         */

        if (m_DialogQueue.Count == 0)
            return;

        ShowNextDialog();
    }

}
