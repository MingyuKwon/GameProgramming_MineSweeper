using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPanel : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public Toggle[] buttons;
 
    public GameObject[] Panels;
    [Header("for Adventure Panel")]
    public TextMeshProUGUI noItem;
    public TextMeshProUGUI trapDamage;

    [Header("for Stage Panel")]
    public TextMeshProUGUI[] StagestartItem;
    public TextMeshProUGUI[] StageWidthHeight;
    public TextMeshProUGUI[] StageTrapTreasure;

    private void UpdateDifficultyPanel()
    {
        int difficulty = (int)StageInformationManager.difficulty;        
        noItem.text = (StageInformationManager.noItemRatio[difficulty] * 100).ToString() + "%";
        trapDamage.text = StageInformationManager.DefaultTrapDamage[difficulty].ToString();
    }

    private void UpdateStagePanel()
    {
        int difficulty = (int)StageInformationManager.difficulty;        
        int stageType = StageInformationManager.currentStagetype;

        StagestartItem[0].text = 0.ToString();
        StagestartItem[1].text = StageInformationManager.StageMagItemAmount[stageType,difficulty].ToString();
        StagestartItem[2].text = 0.ToString();
        StagestartItem[3].text = StageInformationManager.StageModeTime[stageType,difficulty].ToString();

        int width = StageInformationManager.StageModestageWidth[stageType];
        int height = StageInformationManager.StageModestageHeight[stageType];
        StageWidthHeight[0].text = width.ToString();
        StageWidthHeight[1].text = height.ToString();

        float mineRatio = 0;
        switch((Difficulty)difficulty)
        {
            case Difficulty.Easy :
                mineRatio = StageInformationManager.easyMineRatio;
                break;
            case Difficulty.Normal :
                mineRatio = StageInformationManager.normalMineRatio;
                break;
            case Difficulty.Hard :
                mineRatio = StageInformationManager.hardMineRatio;
                break;
            case Difficulty.Professional :
                mineRatio = StageInformationManager.professionalMineRatio;
                break;
        }

        int totalCount = (int)(width * height * mineRatio);
        int mineCount = (int)(totalCount * (1 - StageInformationManager.StageModemineToTreasureRatio[stageType]));
        int treasureCount = totalCount - mineCount;
        StageTrapTreasure[0].text = mineCount.ToString();
        StageTrapTreasure[1].text = treasureCount.ToString();
    }

    private void UpdateTotal()
    {
        UpdateDifficultyPanel();
        UpdateStagePanel();
    }

    private void OnEnable() {
        UpdateDifficulty();
    }


    public void UpdateDifficulty()
    {
        int difficulty = (int)StageInformationManager.difficulty;
        Toggle selectedToggle = buttons[difficulty];
        
        // Toggle Group을 사용하여 선택한 Toggle 활성화
        toggleGroup.SetAllTogglesOff(); // 모든 토글을 끄고
        selectedToggle.isOn = true; // 선택한 토글만 켭니다.
    }
    

    public void ChangeDifficulty(int difficulty)
    {
        StageInformationManager.difficulty = (Difficulty)difficulty;
        UpdateTotal();
    }

    public void ChangeSceneNum(int num)
    {
        StageInformationManager.currentStagetype = num;
        UpdateTotal();
    }
}
