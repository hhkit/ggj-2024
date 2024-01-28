using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SpeechBubble : MonoBehaviour
{
    public string m_Text;
    [SerializeField] private TextMeshProUGUI m_TextUI;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    //[SerializeField] private HorizontalLayoutGroup layoutGroupUI;
    [SerializeField] private RectTransform rectUI;

    float m_Timer = 0;
    const float CHARS_PER_SEC = 8;
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
            .OnUpdate(() =>
            {
                int completeChars = Mathf.FloorToInt((m_Timer - CHAR_FADE_IN_TIME) * CHARS_PER_SEC);
                string text = m_Text.Substring(0, completeChars);

                for (int i = completeChars; i < m_Text.Length; ++i)
                {
                    char c = m_Text[i];
                    float fade = (m_Timer - i / CHARS_PER_SEC) / CHAR_FADE_IN_TIME;
                    if (fade < 0)
                    {
                        if (char.IsWhiteSpace(c)) break; // only end on whitespace so bubble resizes properly
                        text += $"<alpha=#00>{c}";
                        continue;
                    }
                    if (fade < 1.0f)
                        text += $"<alpha=#{(int)(fade * 255):X2}>{c}"; // alpha has no closing tag
                        //text += $"<voffset={fade * 0.1f}em><alpha=#{(int)(fade * 255):X2}>{c}</voffset>"; // alpha has no closing tag
                    else
                                text += c;
                }

                m_TextUI.text = text;
            });
    }

    public Sequence Play(float entryDuration, float exitDuration, float entryOffset, float exitOffset)
    {
        float y = transform.position.y;

        var seq = DOTween.Sequence()
            .Append(m_CanvasGroup.DOFade(0, entryDuration).From())
            .Join(transform.DOMoveY(y, entryDuration).From(y - entryOffset))

            .Append(TweenText())

            .AppendInterval(0.25f)

            .Append(m_CanvasGroup.DOFade(0, exitDuration))
            .Join(transform.DOMoveY(y + exitOffset, entryDuration));
        return seq;
    }
}
