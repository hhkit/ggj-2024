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
    [SerializeField] private Image m_Panel;
    [SerializeField] private HorizontalLayoutGroup layoutGroupUI;
    [SerializeField] private RectTransform rectUI;
    public bool m_IsPlayerBubble;
    static float m_TextAlpha;
    static float m_PanelAlpha;

    void Start()
    {
        m_TextAlpha = m_TextUI.color.a;
        m_PanelAlpha = m_Panel.color.a;
    }
	
    void Update()
    {

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

    public void ResetAlpha()
    {
        Color color = m_Panel.color;
        color.a = m_PanelAlpha;
        m_Panel.color = color;

        Color color2 = m_TextUI.color;
        color2.a = m_TextAlpha;
        m_TextUI.color = color2;
    }

    public void FadeAway(float _time)
    {
        StartCoroutine(FadingAway(_time));
    }

    IEnumerator FadingAway(float _time)
    {
        float timer = _time;
        while (timer > 0)
        {
            Color color = m_Panel.color;
            color.a = m_PanelAlpha / _time * timer * 1.2f;
            m_Panel.color = color;

            Color color2 = m_TextUI.color;
            color2.a = m_TextAlpha / _time * timer * 1.2f;
            m_TextUI.color = color2;

            timer -= Time.deltaTime;
            yield return null;
        }
    }

    public void MoveToPosition(Vector3 _pos, float _time)
    {
        transform.DOMove(_pos, _time);
    }

    public void SetPosition(Vector3 _pos)
    {
        transform.position = _pos;
    }

}
