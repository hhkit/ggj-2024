using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartDayController : MonoBehaviour
{
    public TMPro.TextMeshPro Date;
    public TMPro.TextMeshPro Content;
    
    private DayManager dayManager;
    void Awake()
    {
        Date.text = $"{dayManager.currentDayIndex + 1} August 1XXX";
        Content.text = string.Join("\n\n", dayManager.currentDay.intro);
    }
}
