using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SpeechBubble : MonoBehaviour
{
    public string m_Text;
    [SerializeField] private TextMeshProUGUI m_TextUI;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    //[SerializeField] private HorizontalLayoutGroup layoutGroupUI;
    [SerializeField] private RectTransform rectUI;

    float m_Timer = 0;
    const float CHARS_PER_SEC = 16;
    const float CHAR_FADE_IN_TIME = 0.25f;

    void Awake()
    {
        m_TextUI.SetText("");
    }

    public void SetText(string _text)
    {
        if (_text == null)
            return;
        m_Text = _text;
        //m_TextUI.SetText(_text);
        //Canvas.ForceUpdateCanvases();
        //layoutGroupUI.enabled = false;
        //layoutGroupUI.enabled = true;
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rectUI);
    }

    Tween TweenText()
    {
        float t = m_Text.Length / CHARS_PER_SEC + CHAR_FADE_IN_TIME;
        return DOTween.To(() => m_Timer, v => m_Timer = v, t, t)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                int completeChars = Mathf.CeilToInt((m_Timer - CHAR_FADE_IN_TIME) * CHARS_PER_SEC);
                string text = $"<line-height=100%><voffset=-4>{m_Text.Substring(0, completeChars)}</voffset>";

                for (int i = completeChars; i < m_Text.Length; ++i)
                {
                    char c = m_Text[i];
                    float fade = (m_Timer - i / CHARS_PER_SEC) / CHAR_FADE_IN_TIME;
                    if (fade < 0)
                    {
                        if (char.IsWhiteSpace(c)) break; // only end on whitespace so bubble resizes properly
                        text += $"<voffset=-4><alpha=#00>{c}</voffset>";
                        continue;
                    }
                    if (fade < 1.0f)
                        //text += $"<alpha=#{(int)(fade * 255):X2}>{c}"; // alpha has no closing tag
                        text += $"<voffset={-fade * 4:F6}><alpha=#{(int)(fade * 255):X2}>{c}</voffset>"; // alpha has no closing tag
                    else
                        text += c;
                }

                m_TextUI.text = text;
            });
    }
    Tween TweenPunchline(float exitDuration)
    {
        float t = m_Text.Length / CHARS_PER_SEC + CHAR_FADE_IN_TIME + 1.0f + exitDuration;
        return DOTween.To(() => m_Timer, v => m_Timer = v, t, t)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                string text = $"<line-height=100%>";

                for (int i = 0; i < m_Text.Length; ++i)
                {
                    char c = m_Text[i];
                    float y = -2.0f + Mathf.Sin((i + m_Timer * 8) * Mathf.PI / 4);
                    text += $"<voffset={y:F6}>{c}</voffset>";
                }

                m_TextUI.text = text;
            });
    }

    public Sequence Play(float entryDuration, float exitDuration, float entryOffset, float exitOffset, bool isPunchline, Action PreExit = null)
    {
        float y = transform.position.y;

        var seq = DOTween.Sequence()
            .Append(m_CanvasGroup.DOFade(0, entryDuration).From())
            .Join(transform.DOMoveY(y, entryDuration).From(y - entryOffset))

            .Append(isPunchline ? TweenPunchline(exitDuration) : TweenText())

            .InsertCallback(m_Text.Length / CHARS_PER_SEC + CHAR_FADE_IN_TIME + (isPunchline ? 1.0f : 0.25f), () => PreExit?.Invoke())
            .Join(m_CanvasGroup.DOFade(0, exitDuration))
            .Join(transform.DOMoveY(y + exitOffset, entryDuration));
        return seq;
    }
}
