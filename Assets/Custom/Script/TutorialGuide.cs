using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGuide : MonoBehaviour
{
    public static bool bNowTutorial = false;
    public static bool bTutorialRestart = false;
    public static int tutorialIndex = 0;
    // 0 -> start
    // 1 -> flag
    // 2 -> shovel
    // 3 -> potion
    // 4 -> water
    // 5 -> glass
    // 6 -> crash

    public static bool IsTutoriaShovelEnable()
    {
        return tutorialIndex > 1;
    }

    public static bool IsTutorialPotionEnable()
    {
        return tutorialIndex > 2;
    }

    public static bool IsTutorialMagGlassEnable()
    {
        return tutorialIndex > 4;
    }

    public static bool IsTutorialWaterEnable()
    {
        return tutorialIndex > 3;
    }

    public static bool IsTutorialCrashEnable()
    {
        return tutorialIndex > 5;
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

            case 3:
            FourthTutorialShow();
            break;

            case 4:
            FifthTutorialShow();
            break;

            case 5:
            SixthTutorialShow();
            break;

            case 6:
            SeventhTutorialShow();
            break;
            
        }
        tutorialIndex++;
    }

    void FirstTutorialShow()
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

    void SecondTutorialShow()
    {
        string[] tutorialFirstString = {"", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "자 이제 한번 돌을 선택 한 다음에 오른쪽의 체크 모양 버튼을 눌러서 돌을 치워 봅시다. ";
            tutorialFirstString[1] = "만약 돌을 누르지 않고  체크 모양 버튼을 누른다면 그 위치로 이동 할 수 있습니다";
            break;

            case "English":
            tutorialFirstString[0] = "Now, select a rock and press the checkmark button on the right to remove the rock.";
            tutorialFirstString[1] = "If you press the checkmark button without selecting a rock, you will move to that location.";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

    void ThridTutorialShow()
    {
        string[] tutorialFirstString = {""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "이제 포션을 먹어서 체력을 회복해 봅시다. 우측 포션 그림이 있는 버튼을 눌러 봅시다";
            break;

            case "English":
            tutorialFirstString[0] = "Now, let's recover your health by using a potion. Press the button with the potion icon on the right.";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

    void FourthTutorialShow()
    {
        string[] tutorialFirstString = {"", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "돌을 선택하고, 우측의 파란색 물 그림이 있는 버튼을 눌러 봅시다.";
            tutorialFirstString[1] = "만약 돌 아래에 보물이 있다면 있다는 표현이 나오고, 없다면 없다는 표시가 나옵니다.";
            break;

            case "English":
            tutorialFirstString[0] = "Select a rock and press the button with the blue water icon on the right.";
            tutorialFirstString[1] = "If there is a treasure beneath the rock, it will indicate that it is there. If not, it will show that there is nothing.";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

    void FifthTutorialShow()
    {
        string[] tutorialFirstString = {"", "", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "이제 가장 중요한 돋보기의 사용법을 배워 봅시다";
            tutorialFirstString[1] = "돋보기는 돌이 없는 곳에 사용 할 수 있고, 기존의 보물+지뢰의 개수를 보여주던 곳을 보물 따로, 지뢰 따로 개수를 보여줍니다";
            tutorialFirstString[2] = "금색이 보물이고, 하얀색이 지뢰입니다.";
            break;

            case "English":
            tutorialFirstString[0] = "Now, let's learn how to use the most important tool: the magnifying glass.";
            tutorialFirstString[1] = "The magnifying glass can be used on empty spaces without rocks. It reveals the number of treasures and mines separately, instead of showing their combined total.";
            tutorialFirstString[2] = "Gold indicates treasures, and white represents mines.";           
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }


    void SixthTutorialShow()
    {
        string[] tutorialFirstString = {"", "", ""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "돌을 선택하고 곡괭이 모양 그림이 있는 버튼을 누르면 돌을 깨버릴 수 있습니다.";
            tutorialFirstString[1] = "돌을 깨버리면, 아래에 아무것도 없다면 돌이 없어지고, 아래에 지뢰가 있었다면 지뢰가 사라지게 됩니다!";
            tutorialFirstString[2] = "다만 아래에 보물이 있다면 그 즉시 게임 오버이니 잘 선택하세요!";
            break;

            case "English":
            tutorialFirstString[0] = "Select a rock and press the button with the pickaxe icon to break the rock.";
            tutorialFirstString[1] = "If there's nothing underneath, the rock will simply disappear. If there's a mine, the mine will be removed!";
            tutorialFirstString[2] = "However, if there's a treasure underneath, it's an instant game over, so choose carefully!";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }

    void SeventhTutorialShow()
    {
        string[] tutorialFirstString = {""};

        switch(LanguageManager.currentLanguage)
        {
            case "Korean":
            tutorialFirstString[0] = "이제 모든 설명이 끝났습니다! 자유롭게 게임을 플레이 해보세요!";
            break;

            case "English":
            tutorialFirstString[0] = "Now, all the explanations are complete! Feel free to play the game!";
            break;

        }
         
        EventManager.instance.Invoke_showNoticeUIEvent(tutorialFirstString, true, 1800, 250);
    }
}
