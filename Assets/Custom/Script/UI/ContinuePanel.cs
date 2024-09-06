using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class ContinuePanel : MonoBehaviour
{
    public GameObject NoSaveImage;

    [Space]
    public TextMeshProUGUI potionCount;
    public TextMeshProUGUI magGlassCount;
    public TextMeshProUGUI holyWaterCount;

    [Space]
    public TextMeshProUGUI StageType;
    public TextMeshProUGUI StageLevel;
    public TextMeshProUGUI StageDifficulty;


    [Space]
    [Header("Equipped Panel")]
    public Sprite[] EquippedItemSprites;
    public Image[] EquippedItemImages;

    [Space]
    [Header("Heart Panel")]
    public Sprite[] HeartSprites;
    public Image[] HeartItemImages;

    private void OnEnable() {
        int[] palyerSaveData =  PlayerSaveManager.instance.GetPlayerStageData(); // 크기 13

        if(palyerSaveData == null)
        {
            NoSaveImage.SetActive(true);
            return;
        }else
        {
            NoSaveImage.SetActive(false);
        }

        Item[] playerEquippedItem = new Item[5] {
            (Item)palyerSaveData[7],
            (Item)palyerSaveData[8],
            (Item)palyerSaveData[9],
            (Item)palyerSaveData[10],
            (Item)palyerSaveData[11],
        };
        
        for(int i=0; i<5; i++)
        {
            EquippedItemImages[i].sprite = EquippedItemSprites[0];
        }

        for(int i=0; i<playerEquippedItem.Length && playerEquippedItem[i] != Item.None; i++)
        {
            EquippedItemImages[i].sprite = EquippedItemSprites[(int)playerEquippedItem[i] - 4 + 1];
        }

        int maxHeart = palyerSaveData[2];
        int currentHeart = palyerSaveData[3];

        for(int i=0; i<9; i++)
        {
            HeartItemImages[i].sprite = HeartSprites[0];
        }

        for(int i=0; i<maxHeart; i++)
        {
            HeartItemImages[i].sprite = HeartSprites[1];
        }

        for(int i=0; i<currentHeart; i++)
        {
            HeartItemImages[i].sprite = HeartSprites[2];
        }

        String[] stageTypeTexts = new String[3] {
            "Cave",
            "Crypt",
            "Ruin",
        };

        String[] difficultyTexts = new String[3] {
            "Easy",
            "Normal",
            "Hard",
        };

        StageType.text = stageTypeTexts[palyerSaveData[0]];
        StageLevel.text = "Level" + (palyerSaveData[1] + 1) ;
        StageDifficulty.text = difficultyTexts[palyerSaveData[12]];

        potionCount.text = (palyerSaveData[4] + StageInformationManager.plusPotion_Default_perStage[palyerSaveData[12]]).ToString();
        magGlassCount.text = (palyerSaveData[5] + StageInformationManager.plusMag_Default_perStage[palyerSaveData[12]]).ToString();
        holyWaterCount.text = (palyerSaveData[6] + StageInformationManager.plusHoly_Default_perStage[palyerSaveData[12]]).ToString();

    }
}
