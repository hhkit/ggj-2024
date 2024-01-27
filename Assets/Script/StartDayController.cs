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
        Date.text = $"{dayManager.currentDayIndex + 1} August 1XXX";
        Content.text = "Salutations,\n    " + string.Join("\n    ", dayManager.currentDay.intro);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        LetterClosed.Invoke();
    }
}
