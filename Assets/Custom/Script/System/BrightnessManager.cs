using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager instance;
    public Image panel;
    public static float brightness;

    private void Start() {
        instance = this;
        brightness = PlayerPrefs.GetFloat("brightness", 1);
        setBrightNess(brightness);
    }

    public void setBrightNess(float i)
    {
        brightness = i;
        PlayerPrefs.SetFloat("brightness", brightness);
        PlayerPrefs.Save();
        panel.color = new Color(0,0,0,(1 - brightness) * 0.5f);
    }
}
