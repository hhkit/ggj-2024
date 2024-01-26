using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;

    public GameObject m_JokeWindow;
    void Start()
    {
        
    }
	
    void Update()
    {
        
    }

    public void DisplayJokeWindow(bool _value)
    {
        m_JokeWindow.gameObject.SetActive(_value);
    }
    public void ToggleJokeWindow()
    {
        DisplayJokeWindow(!m_JokeWindow.gameObject.activeSelf);
        
    }
}
