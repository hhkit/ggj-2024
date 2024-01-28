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

    void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        gameSystem = FindObjectOfType<GameSystem>();    

        uiParentInitialPos = uiParent.transform.position;
        // move offscreen
    }

    public void Show()
    {
        canvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        canvas.gameObject.SetActive(false);
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
