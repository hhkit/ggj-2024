using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JesterRandomColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float randLow() => Random.Range(0.0f, 0.2f);
        float randHigh() => Random.Range(0.8f, 1.0f);

        float[] color;
        if (Random.Range(0, 2) == 0)
        {
            color = new float[3] { randLow(), randLow(), randLow() };
            color[Random.Range(0, 3)] = randHigh();
        }
        else
        {
            color = new float[3] { randHigh(), randHigh(), randHigh() };
            color[Random.Range(0, 3)] = randLow();
        }
            
        var randomColor = new Color(color[0], color[1], color[2]);
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = randomColor;
        else
        {
            var img = GetComponent<UnityEngine.UI.Image>();
            if (img != null) img.color = randomColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
