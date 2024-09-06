using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    public static PlayerSaveManager instance = null;
    private int Stagetype = -1;
    private int StageIndex = -1;

    private int MaxHeart = -1;  
    private int CurrentHeart = -1; 

    private int PotionCount = -1; 
    private int MagGlassCount = -1; 
    private int HolyWaterCount = -1;

    private int equippedItem1 = 13; // 13 : None
    private int equippedItem2 = 13; // 13 : None
    private int equippedItem3 = 13; // 13 : None
    private int equippedItem4 = 13; // 13 : None
    private int equippedItem5 = 13; // 13 : None

    private int difficulty = -1;

    private int totalTime = -1;
    private int width = -1;
    private int height = -1;

    private void Awake() {
        instance = this;
    }

    [Button]
    public void ClearPlayerStageData()
    {
        // 전부 저장 안했을 때 초기값으로 저장을 하는 것이 결국 초기화 작업이나 같다
        SavePlayerStageData(new int[16] {
            -1,
            -1, 
            -1,
            -1, 
            -1,
            -1,
            -1,
            13,
            13,
            13,
            13,
            13,
            -1,
            -1,
            -1,
            -1
        }, true);
    }


    [Button]
    public void SavePlayerStageData(int[] paras = null, bool isForce = false)
    {
        if(paras == null) // 만약 인수 없이 호출하면 현재 값들을 기준으로 저장
        {
            int[] returnArrays = StageInformationManager.getHearts();
            if(returnArrays[0] == -1 && !isForce) {
                Debug.Log("Save Deafult");
                
                MaxHeart = 3;
                CurrentHeart = 3;

                Stagetype = StageInformationManager.currentStagetype;
                StageIndex = 0;

                PotionCount = StageInformationManager.Potion_Default; 
                MagGlassCount = StageInformationManager.Mag_Default; 
                HolyWaterCount = StageInformationManager.Holy_Default;

                difficulty = (int)StageInformationManager.difficulty;

                equippedItem1 = (int)Item.None; 
                equippedItem2 = (int)Item.None; 
                equippedItem3 = (int)Item.None; 
                equippedItem4 = (int)Item.None; 
                equippedItem5 = (int)Item.None;

                totalTime = StageInformationManager.DefaultTimeforEntireGame;
                width = StageInformationManager.NextWidth;
                height = StageInformationManager.NextHeight;
            }else
            {
                MaxHeart = returnArrays[0];
                CurrentHeart = returnArrays[1];

                Stagetype = StageInformationManager.currentStagetype;
                StageIndex = StageInformationManager.currentStageIndex;

                returnArrays = StageInformationManager.getUsableItems();
                PotionCount = returnArrays[0]; 
                MagGlassCount = returnArrays[1]; 
                HolyWaterCount = returnArrays[2];

                difficulty = (int)StageInformationManager.difficulty;

                equippedItem1 = (int)EquippedItem.playerEquippedItem[0]; 
                equippedItem2 = (int)EquippedItem.playerEquippedItem[1]; 
                equippedItem3 = (int)EquippedItem.playerEquippedItem[2]; 
                equippedItem4 = (int)EquippedItem.playerEquippedItem[3]; 
                equippedItem5 = (int)EquippedItem.playerEquippedItem[4];

                totalTime = StageInformationManager.NexttotalTime;
                width = StageInformationManager.NextWidth;
                height = StageInformationManager.NextHeight;
            }

        }else // 인수로 호출하면 인수 값으로 저장
        {
            Stagetype = paras[0];
            StageIndex = paras[1];
            MaxHeart = paras[2];  
            CurrentHeart = paras[3]; 
            PotionCount = paras[4]; 
            MagGlassCount = paras[5]; 
            HolyWaterCount = paras[6];
            equippedItem1 = paras[7]; 
            equippedItem2 = paras[8]; 
            equippedItem3 = paras[9]; 
            equippedItem4 = paras[10]; 
            equippedItem5 = paras[11];
            difficulty = paras[12];
            totalTime = paras[13];
            width = paras[14];
            height = paras[15];
        }

        String str = 
                    "====Save===" + LoadingInformation.loadingSceneName + "\n" +
                    "Stagetype : " +  Stagetype + "\n" + 
                    "StageIndex : " +  StageIndex + "\n" + 
                    "MaxHeart : " +  MaxHeart + "\n" + 
                    "CurrentHeart : " +  CurrentHeart + "\n" + 
                    "PotionCount : " +  PotionCount + "\n" + 
                    "MagGlassCount : " +  MagGlassCount + "\n" + 
                    "HolyWaterCount : " +  HolyWaterCount + "\n" + 
                    "difficulty : " +  difficulty + "\n" + 
                    "equippedItem1 : " +  equippedItem1 + "\n" + 
                    "equippedItem2 : " +  equippedItem2 + "\n" + 
                    "equippedItem3 : " +  equippedItem3 + "\n" + 
                    "equippedItem4 : " +  equippedItem4 + "\n" + 
                    "equippedItem5 : " +  equippedItem5 + "\n" 
        ;

        Debug.Log(str);
        

        PlayerPrefs.SetInt("Stagetype", Stagetype);
        PlayerPrefs.SetInt("StageIndex", StageIndex);
        PlayerPrefs.SetInt("MaxHeart", MaxHeart);
        PlayerPrefs.SetInt("CurrentHeart", CurrentHeart);
        PlayerPrefs.SetInt("PotionCount", PotionCount);
        PlayerPrefs.SetInt("MagGlassCount", MagGlassCount);
        PlayerPrefs.SetInt("HolyWaterCount", HolyWaterCount);
        PlayerPrefs.SetInt("equippedItem1", equippedItem1);
        PlayerPrefs.SetInt("equippedItem2", equippedItem2);
        PlayerPrefs.SetInt("equippedItem3", equippedItem3);
        PlayerPrefs.SetInt("equippedItem4", equippedItem4);
        PlayerPrefs.SetInt("equippedItem5", equippedItem5);
        PlayerPrefs.SetInt("difficulty", difficulty);
        PlayerPrefs.SetInt("totalTime", totalTime);
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
        PlayerPrefs.Save();
    }

    public int[] GetPlayerStageData() // 지금 보이는 이 데이터들을 배열로 보내줌
    {
        Stagetype = PlayerPrefs.GetInt("Stagetype", -1);
        StageIndex = PlayerPrefs.GetInt("StageIndex", -1);

        MaxHeart = PlayerPrefs.GetInt("MaxHeart", -1);
        CurrentHeart = PlayerPrefs.GetInt("CurrentHeart", -1);

        PotionCount = PlayerPrefs.GetInt("PotionCount", -1);
        MagGlassCount = PlayerPrefs.GetInt("MagGlassCount", -1);
        HolyWaterCount = PlayerPrefs.GetInt("HolyWaterCount", -1);

        // 만약 여기서 -1을 받는다면, 그건 저장된 값이 아예 없다는 이야기 이다
        if(PotionCount == -1 )
        {
            return null;
        }

        equippedItem1 = PlayerPrefs.GetInt("equippedItem1", 13);
        equippedItem2 = PlayerPrefs.GetInt("equippedItem2", 13);
        equippedItem3 = PlayerPrefs.GetInt("equippedItem3", 13);
        equippedItem4 = PlayerPrefs.GetInt("equippedItem4", 13);
        equippedItem5 = PlayerPrefs.GetInt("equippedItem5", 13);

        difficulty = PlayerPrefs.GetInt("difficulty", -1);

        String str = "====Get=======" +
                    "Stagetype : " +  Stagetype + "\n" + 
                    "StageIndex : " +  StageIndex + "\n" + 
                    "MaxHeart : " +  MaxHeart + "\n" + 
                    "CurrentHeart : " +  CurrentHeart + "\n" + 
                    "PotionCount : " +  PotionCount + "\n" + 
                    "MagGlassCount : " +  MagGlassCount + "\n" + 
                    "HolyWaterCount : " +  HolyWaterCount + "\n" + 
                    "difficulty : " +  difficulty + "\n" + 
                    "equippedItem1 : " +  equippedItem1 + "\n" + 
                    "equippedItem2 : " +  equippedItem2 + "\n" + 
                    "equippedItem3 : " +  equippedItem3 + "\n" + 
                    "equippedItem4 : " +  equippedItem4 + "\n" + 
                    "equippedItem5 : " +  equippedItem5 + "\n" 
        ;

        Debug.Log(str);

        return new int[16] {
            Stagetype,
            StageIndex, 
            MaxHeart,
            CurrentHeart, 
            PotionCount,
            MagGlassCount,
            HolyWaterCount,
            equippedItem1,
            equippedItem2,
            equippedItem3,
            equippedItem4,
            equippedItem5,
            difficulty,
            totalTime,
            width,
            height
        };
    }
}
