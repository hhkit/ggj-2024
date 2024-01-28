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

    public float entryDur = 1f;
    public float exitDur = 2f;
    public float entryOffset = 0.5f;
    public float exitOffset = 1f;
    private struct DialogAction
    {
        public DialogAction(SpeakerId _id, string _text, float _timeToShow, Action _afterShowCallback)
        {
            id = _id;
            timeToShow = _timeToShow;
            text = _text.TrimStart('>');
            afterShowCallback = _afterShowCallback;
        }

        public SpeakerId id;
        public string text;
        public float timeToShow;
        public Action afterShowCallback;
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

#if UNITY_EDITOR
    [Button]
    public void Test(
        SpeakerId who,
        string text,
        float duration)
    {
        PushDialog(who, text, duration);
    }
#endif

    public void PushDialog(
        SpeakerId who, 
        string text,
        float duration,
        Action AfterDisplayCallback = null
    )
    {
        m_DialogQueue.Enqueue(new DialogAction(
            who, text, duration, AfterDisplayCallback));
    }
    public void PushDialog(
        SpeakerId who,
        string[] texts,
        float duration,
        Action AfterDisplayCallback = null
    )
    {
        int count = 0;
        foreach (var text in texts)
        {
            count++;
            var final = count == texts.Length;

            m_DialogQueue.Enqueue(new DialogAction(
                who, text, duration, final ? AfterDisplayCallback : null));
        }
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

        var seq = DOTween.Sequence();

        // entry

        var currY = newBubble.transform.position.y;
        newBubble.transform.position = newBubble.transform.position + Vector3.down * entryOffset;
        newBubble.alpha = 0f;

        seq.Append(newBubble.transform.DOMoveY(currY, entryDur));
        seq.Join(newBubble.DOFade(1f, exitDur));

        // show
        seq.AppendInterval(top.timeToShow);
        if (top.afterShowCallback != null)
            seq.AppendCallback(() => top.afterShowCallback());
        seq.AppendCallback(() => isShowing = false);
        isShowing = true;

        // exit
        var finalY = currY + exitOffset;
        seq.Append(newBubble.transform.DOMoveY(finalY, exitDur));
        seq.Join(newBubble.DOFade(0f, exitDur));

        seq.OnKill(() => Destroy(newBubble.gameObject));

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
