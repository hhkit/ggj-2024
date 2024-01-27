using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            
        GetComponent<SpriteRenderer>().color = new Color(color[0], color[1], color[2]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
