using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGuide : MonoBehaviour
{
    public static bool bNowTutorial = false;
    public static bool bTutorialRestart = false;
    public static int tutorialIndex = 0;

    public static bool IsTutoriaShovelEnable()
    {
        return tutorialIndex > 1;
    }

    public static bool IsTutorialPotionEnable()
    {
        return false;
    }

    public static bool IsTutorialMagGlassEnable()
    {
        return false;
    }

    public static bool IsTutorialWaterEnable()
    {
        return false;
    }

    public static bool IsTutorialCrashEnable()
    {
        return false;
    }


    private void Awake() {
        bTutorialRestart = false;
        tutorialIndex = 0;
    }

    private void OnDestroy() {
        bNowTutorial = false;
    }

    private void OnEnable() {
        EventManager.instance.TutorialShowEvent += TutorialShow;
    }

    private void OnDisable() {
        EventManager.instance.TutorialShowEvent -= TutorialShow;
    }

    private void Start() {
        TutorialShow();
    }

    void TutorialShow()
    {
        switch(tutorialIndex)
        {
            case 0:
            FirstTutorialShow();
            break;

            case 1:
            SecondTutorialShow();
            break;

            case 2:
            ThridTutorialShow();
            break;
        }
        tutorialIndex++;
    }

    void FirstTutorialShow()
    {
        string[] tutorialFirstString = {"", "", "", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "Mine Treasure Sweeper의 튜토리얼에 오신 것을 환영 합니다!. 기본적인 게임의 목표는 숨겨진 보물을 모두 찾는 것입니다!";
            tutorialFirstString[1] = "바닥에 보이는 숫자는 주변 8개의 칸 중에 있는 보물 + 지뢰의 개수 입니다.";
            tutorialFirstString[2] = "다만 숫자만으로는 보물이 몇개인지 지뢰가 몇개인지 알 수 없기에 아이템을 적절히 활용 해야 합니다.";
            tutorialFirstString[3] = "자 그럼 돌을 터치해서 초점을 맞춰 놓고, 오른쪽 제일 상단의 ? 버튼을 눌러 깃발을 세워 봅시다";
            break;

            case "English":
            tutorialFirstString[0] = "Welcome to the tutorial of Mine Treasure Sweeper! The primary goal of the game is to uncover all the hidden treasures!";
            tutorialFirstString[1] = "The numbers on the ground indicate the total number of treasures + mines in the 8 surrounding tiles.";
            tutorialFirstString[2] = "However, since the numbers alone don’t reveal how many are treasures or mines, you’ll need to use items wisely.";
            tutorialFirstString[3] = "Now, touch the rock to focus on it, then press the '?' button at the top-right corner to place a flag.";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

    void SecondTutorialShow()
    {
        string[] tutorialFirstString = {"", "", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "Mine Treasure Sweeper의 튜토리얼에 오신 것을 환영 합니다!. 기본적인 게임의 목표는 숨겨진 보물을 모두 찾는 것입니다!";
            tutorialFirstString[1] = "바닥에 보이는 숫자는 주변 8개의 칸 중에 있는 보물 + 지뢰의 개수 입니다.";
            tutorialFirstString[2] = "다만 숫자만으로는 보물이 몇개인지 지뢰가 몇개인지 알 수 없기에 아이템을 적절히 활용 해야 합니다.";
            break;

            case "English":
            tutorialFirstString[0] = "Welcome to the tutorial of Mine Treasure Sweeper! The primary goal of the game is to uncover all the hidden treasures!";
            tutorialFirstString[1] = "The numbers on the ground indicate the total number of treasures + mines in the 8 surrounding tiles.";
            tutorialFirstString[2] = "However, since the numbers alone don’t reveal how many are treasures or mines, you’ll need to use items wisely.";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

    void ThridTutorialShow()
    {
        string[] tutorialFirstString = {"", "", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "Mine Treasure Sweeper의 튜토리얼에 오신 것을 환영 합니다!. 기본적인 게임의 목표는 숨겨진 보물을 모두 찾는 것입니다!";
            tutorialFirstString[1] = "바닥에 보이는 숫자는 주변 8개의 칸 중에 있는 보물 + 지뢰의 개수 입니다.";
            tutorialFirstString[2] = "다만 숫자만으로는 보물이 몇개인지 지뢰가 몇개인지 알 수 없기에 아이템을 적절히 활용 해야 합니다.";
            break;

            case "English":
            tutorialFirstString[0] = "Welcome to the tutorial of Mine Treasure Sweeper! The primary goal of the game is to uncover all the hidden treasures!";
            tutorialFirstString[1] = "The numbers on the ground indicate the total number of treasures + mines in the 8 surrounding tiles.";
            tutorialFirstString[2] = "However, since the numbers alone don’t reveal how many are treasures or mines, you’ll need to use items wisely.";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

}
