using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuLanguageManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialExplain;
    public TextMeshProUGUI[] menuButtons;
    public TextMeshProUGUI[] stageButtons;
    public TextMeshProUGUI[] modeButtons;


    private string[] menuButtonsTextsKorean = {"튜토리얼", "스테이지 플레이", "설정", "나가기"};
    private string[] menuButtonsTextsEnglish = {"Tutorial", "Stage play", "Setting", "Exit"};

    string[] EnglishStage = {"Cave", "Crypt", "Ruin"};
    string[] KoreanStage = {"동굴", "묘지", "폐허"};

    string[] koreanMode = {"대규모 도굴", "소규모 도굴"};
    string[] englishMode = {"large excavation", "small excavation"};
    


    private string tutorialExplainTextKorean = "이 게임은 보물 + 지뢰찾기 게임입니다. \n\n가이드를 따라서 한 층 한 층 내려가며 게임에 대해 알아보세요! ";
    private string tutorialExplainTextEnglish = "This game is a treasure + minesweeper game. \n\nFollow the guide and learn about the game as you descend floor by floor!";


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
            tutorialExplain.text = tutorialExplainTextEnglish;
            for(int i=0; i<menuButtons.Length; i++)
            {
                menuButtons[i].text = menuButtonsTextsEnglish[i];
            }

            for(int i=0; i<stageButtons.Length; i++)
            {
                stageButtons[i].text = EnglishStage[i];
            }

            for(int i=0; i<modeButtons.Length; i++)
            {
                modeButtons[i].text = englishMode[i];
            }
            
        }else
        {
            tutorialExplain.text = tutorialExplainTextKorean;
            for(int i=0; i<menuButtons.Length; i++)
            {
                menuButtons[i].text = menuButtonsTextsKorean[i];
            }

            for(int i=0; i<stageButtons.Length; i++)
            {
                stageButtons[i].text = KoreanStage[i];
            }

            for(int i=0; i<modeButtons.Length; i++)
            {
                modeButtons[i].text = koreanMode[i];
            }
        }
    }
}
