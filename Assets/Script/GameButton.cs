using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<UnityEngine.UI.Button>().enabled)
            AudioManager.PlayOneShot("MouseDown");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<UnityEngine.UI.Button>().enabled)
            AudioManager.PlayOneShot("MouseOver");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GetComponent<UnityEngine.UI.Button>().enabled)
            AudioManager.PlayOneShot("MouseUp");
    }
    
}
