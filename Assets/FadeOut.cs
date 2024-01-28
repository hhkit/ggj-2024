using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Color start;
    public Color end;
    public Image bg;

    public float fadeTime;
    float currTime;
    // Start is called before the first frame update
    void Start()
    {
        currTime = fadeTime;
        bg.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(currTime > 0)
        {
            currTime -= Time.deltaTime;
            bg.color  = Color.Lerp(end,start,currTime);
        }
    }

    public void Reset()
    {
        currTime = fadeTime;
    }
}
