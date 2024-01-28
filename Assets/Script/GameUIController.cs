using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Transform uiParent;
    Vector3 uiParentInitialPos;

    Canvas canvas { get => GetComponent<Canvas>(); }
    Button[] buttons;

    bool isTransitioning = false;
    GameSystem gameSystem;

    public Color disabledColour;
    public Color enabledColour;

    public List<GameObject> buttonObjs = new List<GameObject>();

    void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        gameSystem = FindObjectOfType<GameSystem>();    

        uiParentInitialPos = uiParent.transform.position;
        // move offscreen
        Hide();
    }

    public void Show()
    {
        //canvas.gameObject.SetActive(true);
        foreach(GameObject obj in buttonObjs)
        {
            obj.GetComponent<Image>().color = enabledColour;
            obj.GetComponent<Button>().enabled = true;
        }
    }

    public void Hide()
    {
        //Grey out, disable
        //canvas.gameObject.SetActive(false);
        foreach(GameObject obj in buttonObjs)
        {
            obj.GetComponent<Image>().color = disabledColour;
            obj.GetComponent<Button>().enabled = false;
        }
    }

    public void OnAcceptClick()
    {
        Hide();
        gameSystem.AcceptJester();
    }

    public void OnRepeatConversation()
    {
        Hide();
        gameSystem.ReplayJesterConversation();
    }

    public void OnDeclineClick()
    {
        Hide();
        gameSystem.RejectJester();
    }
}
