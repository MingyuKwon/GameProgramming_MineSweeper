using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuLanguageManager : MonoBehaviour
{
    public TextMeshProUGUI[] menuButtons;
    public TextMeshProUGUI[] stageButtons;

    private string[] menuButtonsTextsKorean = {"스테이지 플레이","나가기"};
    private string[] menuButtonsTextsEnglish = {"Stage play","Exit"};

    string[] EnglishStage = {"Cave", "Crypt", "Ruin"};
    string[] KoreanStage = {"동굴", "묘지", "폐허"};

    private void OnEnable() {
        LanguageManager.languageChangeEvent += UpdatePanel;
        UpdatePanel("");
    }

    private void OnDisable() {
        LanguageManager.languageChangeEvent -= UpdatePanel;
    }

    private void UpdatePanel(string s)
    {
        if(LanguageManager.currentLanguage == "English")
        {
            for(int i=0; i<menuButtons.Length; i++)
            {
                menuButtons[i].text = menuButtonsTextsEnglish[i];
            }

            for(int i=0; i<stageButtons.Length; i++)
            {
                stageButtons[i].text = EnglishStage[i];
            }
            
        }else
        {
            for(int i=0; i<menuButtons.Length; i++)
            {
                menuButtons[i].text = menuButtonsTextsKorean[i];
            }

            for(int i=0; i<stageButtons.Length; i++)
            {
                stageButtons[i].text = KoreanStage[i];
            }


        }
    }
}
