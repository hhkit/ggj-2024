using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystem : MonoBehaviour
{
    public string m_GameSceneName;
    void Start()
    {
        
    }
	
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(m_GameSceneName);
    }
}
