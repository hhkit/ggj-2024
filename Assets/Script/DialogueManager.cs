using DG.Tweening;
#if UNITY_EDITOR
using EasyButtons;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent<bool> OnRunninStatusChange;
    public Action OnJesterSpeakJoke;

    public float entryDur = 1f;
    public float exitDur = 2f;
    public float entryOffset = 0.5f;
    public float exitOffset = 1f;
    private struct DialogAction
    {
        public DialogAction(SpeakerId _id, string _text, bool _isPunchline, Action _exitCallback)
        {
            id = _id;
            text = _text.TrimStart('>');
            isPunchline = _isPunchline;
            exitCallback = _exitCallback;
        }

        public SpeakerId id;
        public string text;
        public bool isPunchline;
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

    public void PushDialog(
        SpeakerId who, 
        string text,
        bool isPunchline,
        Action exitCallback = null
    )
    {
        m_DialogQueue.Enqueue(new DialogAction(
            who, text, isPunchline, exitCallback));
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
                    AudioManager.PlayOneShot("PlayerReply");
                    AudioManager.PlayOneShot("SpeechBubbleOpen", 0.5f,1.6f);
                    break;
                case SpeakerId.King:
                    bubblePrefab = m_KingBubble;
                    break;
                case SpeakerId.Jester:
                    bubblePrefab = m_JesterBubble;
                    AudioManager.PlayOneShot("JesterOpenerSound");
                    AudioManager.PlayOneShot("SpeechBubbleOpen", 0.5f, 1.6f);
                    OnJesterSpeakJoke?.Invoke();
                    break;
            }

            Debug.Assert(bubblePrefab != null);
        }

        var newBubble = Instantiate(bubblePrefab);
        newBubble.gameObject.SetActive(true);
        newBubble.SetText(top.text);

        var seq = newBubble.Play(entryDur, exitDur, entryOffset, exitOffset, top.isPunchline,
            () => {
                top.exitCallback?.Invoke();
                isShowing = false;
            });

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

    bool isShowingPrev = false;

    private void LateUpdate()
    {
        var isShowingCurr = isShowing;

        if (isShowingCurr != isShowingPrev)
            OnRunninStatusChange.Invoke(isShowing);

        isShowingPrev = isShowingCurr;
    }

}
