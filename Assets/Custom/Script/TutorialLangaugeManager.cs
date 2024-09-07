using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialLangaugeManager : MonoBehaviour
{
    public TextMeshProUGUI[] tutorial1Title;
    public TextMeshProUGUI[] tutorial1Text;

    public TextMeshProUGUI[] tutorial2Title;
    public TextMeshProUGUI[] tutorial2Text;

    public TextMeshProUGUI[] tutorial3Title;
    public TextMeshProUGUI[] tutorial3Text;

    public TextMeshProUGUI[] tutorial4Title;
    public TextMeshProUGUI[] tutorial4Text;


    private string[] tutorial1TitleStringEnglish = {
    "Basics of the Game",
    "Game Clear",
    "Numbers on the Floor",
    "Game Over",
    };
    private string[] tutorial1TextStringEnglish = {
    "The goal is to find all the treasures on each stage to proceed to the next level, reaching the final floor.",
    "Underneath the rocks, there might be treasures to find or traps that can cause damage. We must try to avoid traps and collect all treasures.",
    "This means that the sum of traps + treasures in the 8 surrounding tiles is 3. The exact number of each is unknown.",
    "The left white number indicates the number of traps among the 8 surrounding tiles, while the right yellow number indicates the number of treasures.",
    "There are three conditions for game over:",
    "If you fall into a trap, your health decreases, and the game is over when health reaches 0. Be careful, as trap damage increases with difficulty and levels.",
    "If the time limit is exceeded, it's game over.",
    "Removing a treasure while trying to clear a trap results in game over.",
    };
    private string[] tutorial2TitleStringEnglish = {
    "Mouse Cursor",
    "Left Click",
    "Wheel Click",
    "Right Click",
    };
    private string[] tutorial2TextStringEnglish = {
    "Displays the item window at the cursor position. Click the wheel again to close it.",
    "This shows a highlighted line at the mouse position. The mouse cursor is activated only in places where the player can move.",
    "Left-clicking the mouse allows you to move according to the cursor's position or clear a rock.",
    "If the cursor is in a spot without a rock, you can move to that position.",
    "If the cursor is on a rock and the rock is right next to the player, you can dig out the rock to check what's underneath.",
    "Right-clicking the mouse allows you to place a flag on the rock at the cursor's position. The flag changes shape each time you right-click.",
    "You can mark it if you're unsure whether something under a rock is a trap or a treasure.",
    "You can mark the presence of a trap under a rock.",
    };
    private string[] tutorial3TitleStringEnglish= {
    "Useable Items",
    "Item Description",
    };
    private string[] tutorial3TextStringEnglish = {
    "These are consumable items that can be used to complete stages. The quantity increases each time you complete a stage or find a treasure.",
    "Can be used at the player's location. Recovers 1 health.",
    "Before using magnifying glass",
    "After using magnifying glass",
    "Can be used at the player's location with a number. It separates the combined (treasure + trap) number into individual counts.",
    "Can be used on rocks near the player. Determines whether there is a treasure under the rock. However, if it indicates no treasure, it's unclear whether it's empty or a trap.",
    "There is a treasure",
    "There is no treasure",
    "Can be used on rocks near the player. Removes the rock, including any traps underneath. Unlimited use, but removing a treasure results in game over.",
    };
    private string[] tutorial4TitleStringEnglish = {
    "Equipment Items",
    "Item Description",
};
    private string[] tutorial4TextStringEnglish = {
    "These are items you can obtain each time you clear a stage.",
    "Increases maximum health by 3.",
    "Increases the number of potions provided when you clear a stage.",
    "Increases the number of potions found in treasures.",
    "Increases the number of Holy Waters provided when you clear a stage.",
    "Increases the number of Holy Waters found in treasures.",
    "Increases the number of Magnifying Glasses provided when you clear a stage.",
    "Increases the number of Magnifying Glasses found in treasures.",
    "Increases the additional time provided when you clear a stage.",
    "Increases the number of consumable items found in treasures.",
};

    private string[] tutorial1TitleStringKorean = {
        "게임의 기초",
        "게임 클리어",
        "바닥의 숫자",
        "게임 오버",
    };
    private string[] tutorial1TextStringKorean = {
        "각 스테이지에 있는 모든 보물을 찾아내 다음 층으로 넘어가, 최종 층에 도달하는 것이 목표입니다",
        "바위 아래에는 찾아야 하는 보물이나, 피해를 입을 수 있는 함정이 있을 수도 있습니다. 우리는 최대한 함정을 피해서 보물을 전부 얻어야 합니다.",
        "바닥 주변 8개의 칸에 함정 + 보물의 개수가 3개가 된다는 것을 의미합니다. 각각 몇개가 있는지는 알 수 없습니다 ",
        "왼쪽 휜색 숫자가 주변 8칸 중 함정의 개수,  오른쪽 노란색 숫자가 보물의 개수입니다.",
        "게임 오버가 되는 조건은 3가지가 있습니다 ",
        "함정에 걸리면 체력이 줄어듭고, 그러다 체력이 0이 되면 게임 오버가 됩니다. 난이도와 층이  올라감에 따라 함정의 데미지가 올라가니 주의합시다.",
        "시간제한을 넘기게 되면 게임 오버가 됩니다.",
        "함정을 제거하려다 보물을 제거하면 게임 오버입니다.",
    };
    private string[] tutorial2TitleStringKorean = {
        "마우스 커서",
        "좌클릭",
        "휠클릭",
        "우클릭",
    };
    private string[] tutorial2TextStringKorean = {
        "커서 위치에 아이템 창을 띄웁니다 . 다시 닫고 싶으면 휠을 다시 클릭하면 됩니다 ",
        "마우스 위치에 표시 되는 강조 선입니다. 마우스 커서는 플레이어가 이동해서 갈 수 있는 곳에만 활성화 됩니다 ",
        "마우스 좌클릭을 하면 커서의 위치에 따라 이동하거나, 바위를 치울 수 있습니다 ",
        "커서가 바위가 없는 곳에 있다면, 해당 위치로 이동 할 수 있습니다",
        "커서가 바위 위치에 있고, 바위가 플레이어 바로 옆에 있다면, 바위를 퍼내 바위 밑에 뭐가 있는지 확인 할 수 있습니다 ",
        "마우스 우클릭을 하면 커서의 위치의 바위에 깃발을 세울 수 있습니다. 깃발은 우클릭을 누를 때 마다 모양이 바뀝니다 ",
        "바위 아래 뭔가 있지만, 함정인지 보물인지 모르겠는 경우 표시 할 수 있습니다 ",
        "바위 아래에 함정이 있는 것을 표시할 수 있습니다.",
    };
    private string[] tutorial3TitleStringKorean= {
        "사용 아이템",
        "아이템 설명",
    };
    private string[] tutorial3TextStringKorean= {
        "스테이지를 깨기 위해서 사용 할 수 있는 소비형 아이템 입니다. 각 스테이지를 완료하거나, 보물을 찾을 때 마다 개수가 추가됩니다.",
        "플레이어 위치에서 사용 가능. 체력 1을 회복합니다 ",
        "돋보기 사용 전 ",
        "돋보기 사용 후 ",
        "숫자가 적혀 있는 플레이어 위치에서 사용 가능. 기존에 숫자가 (보물 + 함정) 통합되어서 보이던 것이 분리되서 보이게 만들어 줍니다 ",
        "플레이어 주변 바위에 사용 가능. 바위 아래에 보물이 있는지 없는지를 판별 합니다. 다만, 보물이 없다고 표시 된 경우에, 아무것도 없는 것인지, 함정이 있는 것인지는 알 수 없습니다.",
        "보물이 있습니다 ",
        "보물이 없습니다 ",
        "플레이어 주변 바위에 사용가능. 바위를 아래에 있는 함정을 포함해서 제거합니다. 무제한으로 사용할 수 있는 대신, 보물을 제거하게 되면 게임 오버 입니다",
    };
    private string[] tutorial4TitleStringKorean = {
        "장비 아이템",
        "아이템 설명",
    };
    private string[] tutorial4TextStringKorean = {
        "스테이지를 깰 때 마다 얻을 수 있는 아이템 입니다 ",
        "최대 체력을 3 올립니다 ",
        "스테이지를 깰 때 지급되는 포션의 개수를 늘립니다 ",
        "보물에서 포션을 찾을 때 얻는 개수를 늘립니다 ",
        "스테이지를 깰 때 지급되는 성수의 개수를 늘립니다 ",
        "보물에서 성수를 찾을 때 얻는 개수를 늘립니다 ",
        "스테이지를 깰 때 지급되는 돋보기의 개수를 늘립니다 ",
        "보물에서 돋보기를 찾을 때 얻는 개수를 늘립니다 ",
        "스테이지를 깰 때 지급되는 추가시간이 증가합니다 ",
        "보물에서 소비 아이템을 찾을 때 얻는 개수를 늘립니다 ",
    };

    private void OnEnable() {
        UpdatePanel("");
    }

    private void UpdatePanel(string s)
    {
        string[] tutorial1TitleString = tutorial1TitleStringEnglish;
        string[] tutorial1TextString  = tutorial1TextStringEnglish;
        string[] tutorial2TitleString = tutorial2TitleStringEnglish;
        string[] tutorial2TextString  = tutorial2TextStringEnglish;
        string[] tutorial3TitleString = tutorial3TitleStringEnglish;
        string[] tutorial3TextString  = tutorial3TextStringEnglish;
        string[] tutorial4TitleString = tutorial4TitleStringEnglish;
        string[] tutorial4TextString  = tutorial4TextStringEnglish;
        if(LanguageManager.currentLanguage == "English")
        {
            tutorial1TitleString = tutorial1TitleStringEnglish;
            tutorial1TextString  = tutorial1TextStringEnglish;
            tutorial2TitleString = tutorial2TitleStringEnglish;
            tutorial2TextString  = tutorial2TextStringEnglish;
            tutorial3TitleString = tutorial3TitleStringEnglish;
            tutorial3TextString  = tutorial3TextStringEnglish;
            tutorial4TitleString = tutorial4TitleStringEnglish;
            tutorial4TextString  = tutorial4TextStringEnglish;
        }else
        {
            tutorial1TitleString = tutorial1TitleStringKorean;
            tutorial1TextString  = tutorial1TextStringKorean;
            tutorial2TitleString = tutorial2TitleStringKorean;
            tutorial2TextString  = tutorial2TextStringKorean;
            tutorial3TitleString = tutorial3TitleStringKorean;
            tutorial3TextString  = tutorial3TextStringKorean;
            tutorial4TitleString = tutorial4TitleStringKorean;
            tutorial4TextString  = tutorial4TextStringKorean;
        }

        for(int i=0; i<tutorial1Title.Length; i++)
        {
            tutorial1Title[i].text = tutorial1TitleString[i];
        }
        for(int i=0; i<tutorial1Text.Length; i++)
        {
            tutorial1Text[i].text = tutorial1TextString[i];
        }
        for(int i=0; i<tutorial2Title.Length; i++)
        {
            tutorial2Title[i].text = tutorial2TitleString[i];
        }
        for(int i=0; i<tutorial2Text.Length; i++)
        {
            tutorial2Text[i].text = tutorial2TextString[i];
        }
        for(int i=0; i<tutorial3Title.Length; i++)
        {
            tutorial3Title[i].text = tutorial3TitleString[i];
        }
        for(int i=0; i<tutorial3Text.Length; i++)
        {
            tutorial3Text[i].text = tutorial3TextString[i];
        }
        for(int i=0; i<tutorial4Title.Length; i++)
        {
            tutorial4Title[i].text = tutorial4TitleString[i];
        }
        for(int i=0; i<tutorial4Text.Length; i++)
        {
            tutorial4Text[i].text = tutorial4TextString[i];
        }

    }

}
