using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeIOText : MonoBehaviour
{

    public TMP_Text text;
    float opacity = 100;
    int dir = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        opacity += dir * 100 * Time.deltaTime;
        if (opacity >= 255f)
        {
            opacity = 255f;
            dir = -1;
        }
        else if (opacity <= 0f)
        {
            opacity = 0f;
            dir = 1;
        }
        Color color = new Color32(255, 255, 255, (byte)opacity);
        text.color = color;
    }
}
