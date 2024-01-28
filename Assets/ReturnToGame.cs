using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToGame : MonoBehaviour
{
    public string sceneName;
    public void ReturnToGameScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
