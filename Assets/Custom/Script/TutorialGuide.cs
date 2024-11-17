using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGuide : MonoBehaviour
{
    public static bool bNowTutorial = false;
    public static bool bTutorialRestart = false;

    private void Awake() {
        bNowTutorial = true;
        bTutorialRestart = false;
    }

    private void OnDestroy() {
        bNowTutorial = false;
    }

    void Start()
    {
        string[] temp = {"Hello Boys and girls get up get up get up get up get up get up get up get up get up get up get up get up get up get up ", "111111111111111111111111111111111111111111111111 "};
        EventManager.instance.Invoke_showNoticeUIEvent(temp, true, 1800, 250);
    }

    void Update()
    {
        
    }
}
