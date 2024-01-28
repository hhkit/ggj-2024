using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using YamlDotNet.Core.Tokens;
using Unity.VisualScripting;

public class SpeechBubble : MonoBehaviour
{
    public string m_Text;
    [SerializeField] private TextMeshProUGUI m_TextUI;
    [SerializeField] private Image m_Panel;
    [SerializeField] private HorizontalLayoutGroup layoutGroupUI;
    [SerializeField] private RectTransform rectUI;

    public float alpha
    {
        get => m_Panel.color.a;
        set {
            m_Panel.color = m_Panel.color.WithAlpha(value);
            m_TextUI.color = m_TextUI.color.WithAlpha(value);
        }
    }

    public void SetText(string _text)
    {
        if (_text == null)
            return;
        m_TextUI.SetText(_text);
        //Canvas.ForceUpdateCanvases();
        //layoutGroupUI.enabled = false;
        //layoutGroupUI.enabled = true;
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rectUI);
    }

    public Tween DOFade(float alpha, float duration)
    {
        var seq = DOTween.Sequence();
        seq.Join(m_TextUI.DOFade(alpha, duration));
        seq.Join(m_Panel.DOFade(alpha, duration));
        return seq;
    }
}
