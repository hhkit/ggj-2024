using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ToGoodEnd : MonoBehaviour
{
    public string sceneName;
    public bool rCheck,uCheck,nCheck = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            rCheck = true;
        }

        if(rCheck)
        {
            if(Input.GetKeyUp(KeyCode.U))
            {
                uCheck = true;
            }
        }

        if(uCheck)
        {
            if(Input.GetKeyUp(KeyCode.N))
            {
                nCheck = true;
            }
        }

        if(rCheck&&uCheck&&nCheck)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    void Reset()
    {
        rCheck = false;
        uCheck = false;
        nCheck = false;
    }
}
