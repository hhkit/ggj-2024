using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.PlayOneShot("MouseDown");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.PlayOneShot("MouseOver");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.PlayOneShot("MouseUp");
    }
    
}
