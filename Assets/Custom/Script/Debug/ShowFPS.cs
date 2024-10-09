using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public TextMeshProUGUI FpsShowText;

    void Start()
    {
        
    }

    void Update()
    {
        if(FpsShowText)
        {
            float fps = 1.0f / Time.deltaTime;
            FpsShowText.text = "FPS: " + Mathf.RoundToInt(fps).ToString();
        }
    }

}
