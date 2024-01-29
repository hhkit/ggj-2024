using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StartDayController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Date;
    public TMPro.TextMeshProUGUI Content;
    public UnityEvent LetterClosed;


    private DayManager dayManager;

    void Awake()
    {
        // must be attached to the canvas
        Debug.Assert(GetComponent<Canvas>() != null); 

        dayManager = FindObjectOfType<DayManager>();
        Date.text = $"{dayManager.currentDayIndex + 1} August 1xxx";
        Content.text = "Salutations,\n    " + string.Join("\n    ", dayManager.currentDay.intro) + "\nOrders:  " + dayManager.currentDay.orders;
    }

    private void Start()
    {
        transform.GetChild(0).GetComponent<RectTransform>().DOAnchorMin(new Vector2(0.5f, -1), 1.0f).From().SetEase(Ease.OutExpo);
        transform.GetChild(0).GetComponent<RectTransform>().DOAnchorMax(new Vector2(0.5f, 0), 1.0f).From().SetEase(Ease.OutExpo);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        LetterClosed.Invoke();
    }
}
