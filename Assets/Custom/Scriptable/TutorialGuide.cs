using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGuide : MonoBehaviour
{
    public int tutorialIndex;
    public bool tutorialRest;

    [Space]
    [Space]
    [Space]
    [Space]
    public Line tutorialLine;
    public Line tutorialLineEnglish;

    Line currentLine;
    int stringCount;
    public static int tutorialTextindex = 0;

    void Start()
    {
        tutorialTextindex = 0;
        currentLine = tutorialLineEnglish;

        stringCount = currentLine.stringCount;

        EventManager.instance.TutorialTextTriggerEvent += TutorialShow;

        EventManager.instance.Invoke_TutorialTextTriggerEvent();
    }

    private void OnDisable() {
        EventManager.instance.TutorialTextTriggerEvent -= TutorialShow;
        tutorialTextindex = 0;
    }

    private void TutorialShow()
    {
        if(LanguageManager.currentLanguage == "English")
        {
            currentLine = tutorialLineEnglish;
        }else
        {
            currentLine = tutorialLine;
        }
        stringCount = currentLine.stringCount;

        if( tutorialTextindex < stringCount  && tutorialTextindex == 0)
        {
            EventManager.instance.Invoke_showNoticeUIEvent(currentLine.line1, true, 1800, 250);
            Sceneario(tutorialTextindex);
        }

        if( tutorialTextindex < stringCount  && tutorialTextindex == 1)
        {
            EventManager.instance.Invoke_showNoticeUIEvent(currentLine.line2, true, 1800, 250);
            Sceneario(tutorialTextindex);
        }

        if( tutorialTextindex < stringCount  && tutorialTextindex == 2)
        {
            EventManager.instance.Invoke_showNoticeUIEvent(currentLine.line3, true, 1800, 250);
            Sceneario(tutorialTextindex);
        }

        if( tutorialTextindex < stringCount  && tutorialTextindex == 3)
        {
            EventManager.instance.Invoke_showNoticeUIEvent(currentLine.line4, true, 1800, 250);
            Sceneario(tutorialTextindex);
        }

        if( tutorialTextindex < stringCount  && tutorialTextindex == 4)
        {
            EventManager.instance.Invoke_showNoticeUIEvent(currentLine.line5, true, 1800, 250);
            Sceneario(tutorialTextindex);
        }

        tutorialTextindex++;
    }

    private void Sceneario(int index)
    {
        Tutorial1Scenario(index);
        Tutorial1RestScenario(index);
        Tutorial2Scenario(index);
    }

    private void Tutorial1Scenario(int index)
    {
        if(tutorialRest || tutorialIndex != 0) return;

        switch(index)
        {
            case 0 :
                InputManager.itemLock = true;
                InputManager.flagLock = true;
                InputManager.shovelLock = true;
                break;
            case 1 :
                InputManager.flagLock = false;
                break;
            case 2 :
                InputManager.shovelLock = false;
                break;
            case 3 :
                break;
            case 4 :
                break;
        }
    }

    private void Tutorial2Scenario(int index)
    {
        if(tutorialRest || tutorialIndex != 1) return;

        switch(index)
        {
            case 0 :
                InputManager.itemLock = false;
                break;
            case 1 :
                break;
            case 2 :
                break;
            case 3 :
                break;
            case 4 :
                break;
        }
    }

    private void Tutorial1RestScenario(int index)
    {
        if(!tutorialRest || tutorialIndex != 0) return;

        switch(index)
        {
            case 0 :
                InputManager.itemLock = true;
                break;
            case 1 :
                break;
            case 2 :
                break;
            case 3 :
                break;
            case 4 :
                break;
        }
    }

}
