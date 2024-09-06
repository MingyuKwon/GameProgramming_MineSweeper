using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class InformationUI : MonoBehaviour
{
    public enum InformationSituation{
        startItemExplain = 0,
        defaultPlusItemExplain = 1,
        ItemPlusItemExplain = 2,
        item_obtain_Up_PercentageExplain = 3,
        TrapDamageExplain = 4,
        NoItemExplain = 5,
        TutorialExplain = 6,
        StageExplain = 7,
        Setting = 8,
        GameExit = 9,
        PerStage = 10,
        Left = 11,
        Minimap = 12,
        Menu = 13,
        Restart = 14,
        TutorialPanel = 15,
        GoBackMainMenu = 16,
    }

    string[] koreanExplations = {
        "새로운 게임을 시작하면 기본 주어지는 아이템의 개수 입니다.",
        "스테이지마다 기본으로 추가되는 아이템의 개수 입니다. ",
        "특정 장비 아이템이 있을 경우, 스테이지마다 추가되는 아이템의 개수가 증가하는 양입니다 ",
        " 특정 장비 아이템이 있을 경우, 보물을 열어서 아이템을 얻게 될 때, 그 아이템의 개수가 1보다 커질 확률입니다. ",
        "함정에 빠지게 될 때 얻게 되는 피해의 양입니다",
        "보물을 열었을 때, 아이템이 없을 확률입니다.",
        "튜토리얼 스테이지를 플레이하며 게임에 대해 알아 보세요!\n 튜토리얼 스테이지는 한 스테이지를 제외하면 효율적인 게임 가이드를 위해 구조가 고정되어있습니다",
        "본격적인 게임 플레이 모드입니다.\n\n 대규모 도굴 - 한층 한층 내려가며 장비 아이템을 얻으면서 최하층에 도달해서 보물을 얻는 것이 목표입니다. 각종 아이템은 랜덤으로 나옵니다 \n\n 소규모 도굴 - 하나의 스테이지만 빠르게 클리어 하는 것이 목적입니다. 아이템은 돋보기로 고정입니다" ,
        "게임의 설정을 바꿉니다",
        "게임을 종료합니다",
        "새로운 스테이지가 시작할 떄 자동으로 주어지는 아이템의 개수와, 시간입니다.\n\n 난이도, 현재 보유 장비 아이템에 영향을 받아서 바뀌게 됩니다",
        "현재 스테이지에 남아있는 보물, 함정의 개수입니다\n\n 스테이지 전체 크기에 영향을 받으며, 보물을 얻거나 함정을 파괴하면 그 개수들이 줄어듭니다.",
        "미니맵을 열어서, 스테이지 전체를 봅니다",
        "메뉴 창을 열어서 플레이어 상태와 아이템 설명을 자세히 볼 수 있습니다. \n\n 또한 재시작, 튜토리얼, 설정, 메인 메뉴 돌아가기 중 하나를 선택 할 수 있습니다.",
        "스테이지를 재시작합니다. \n\n 만약 대규모 도굴이면 해당 스테이지의 레벨 1으로 돌아가고, 소규모 도굴이면 해당 스테이지를 재시작합니다",
        "튜토리얼 가이드를 보여줍니다 \n\n 만약 튜토리얼 모드를 플레이하지 않았거나, 잊었다면 여기에서 다시 보실 수 있습니다",
        "메인 메뉴로 돌아갑니다 \n\n 저장하지 않은 데이터는 날아가게 됩니다",
    };
    string[] englishExplations = {
        "This is the number of items that are initially given when starting a new game.",
        "This is the number of items that are added by default in each stage",
        "This is the amount by which the number of items added in each stage increases if a specific equipment item is present",
        "If a specific equipment item is present, this is the probability that the number of items obtained when opening a treasure will be greater than one",
        "This is the amount of damage incurred when falling into a trap",
        "This is the probability of not finding any items when opening a treasure",
        "Play through the tutorial stage to learn about the game!\n Except for one stage, the structure of the tutorial stage is fixed for an efficient game guide.",
        "This is the full-scale game play mode.\n\n- Large-scale excavation: The goal is to descend floor by floor, acquiring equipment items, and reach the bottom floor to obtain the treasure. Items appear randomly.\n\n- Small-scale excavation: The aim is to quickly clear a single stage. Items are fixed to magnifying glasses.",
        "Change the game settings.",
        "Exit the game.",
        "This is the number of items automatically given and the time when a new stage starts.\n\n It will change based on the difficulty and the current equipment items you have.",
        "This indicates the number of remaining treasures and traps in the current stage.\n\n It is influenced by the overall size of the stage, and these numbers decrease as you acquire treasures or destroy traps.",
        "Open the minimap to view the entire stage.",  
        "Open the menu window to see detailed information about the player's status and item descriptions.\n\n Additionally, you can choose to restart, go to the tutorial, access settings, or return to the main menu.",
        "Restarts the stage. \n\n If it's a large-scale excavation, you will return to level 1 of that stage, and if it's a small-scale excavation, the stage will be restarted.",
        "Shows the tutorial guide. \n\n If you haven't played the tutorial mode or have forgotten it, you can view it again here.",
        "Returns to the main menu. \n\n Any unsaved data will be lost.",


    };

    public TextMeshProUGUI MainText;
    public static InformationUI instance = null;

     private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }

        gameObject.SetActive(false);
    }

    public void ShowInformation(InformationSituation situation)
    {
        if(LanguageManager.currentLanguage == "Korean")
        {
            MainText.text = koreanExplations[(int)situation];
        }else if(LanguageManager.currentLanguage == "English")
        {
            MainText.text = englishExplations[(int)situation];
        }
        
        gameObject.SetActive(true);
    }

    public void CloseInformationPanel()
    {   
        gameObject.SetActive(false);
        MainText.text = "";
    }
}
