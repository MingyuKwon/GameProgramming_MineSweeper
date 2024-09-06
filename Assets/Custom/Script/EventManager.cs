using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EventType {
    MineAppear = 0,
    MineDisappear = 1,
    TreasureAppear = 2,
    TreasureDisappear = 3,
    Set_Width_Height = 4,
    Set_Heart = 5,
    Game_Over = 6,
    Game_Restart = 7,
    Item_Use = 8,
    Item_Obtain = 9,
    None = 10
}

public enum GameOver_Reason {
    None = 0,
    Heart0 = 1,
    TreasureCrash = 2,
    TimeOver = 3,
}

public enum Item {
    
    Potion = 1,
    Mag_Glass = 2,
    Holy_Water = 3,

    Potion_Plus = 4,

    Glass_Plus =5,

    Water_Plus = 6,

    Potion_PercentageUP = 7,

    Glass_PercentageUP = 8,

    Water_PercentageUP = 9,

    ALL_PercentageUP = 10,

    Time_Plus = 11,

    Heart_UP = 12,
    None = 13,
}

public enum ItemUseType {
    Shovel = 4,
    Potion = 3,
    Mag_Glass = 2,
    Holy_Water = 0,
    Crash = 1,
}

public class EquippedItem
{
    public static Item[] playerEquippedItem = new Item[5] {Item.None,Item.None,Item.None,Item.None,Item.None };
    public static Item nextObtainItem = Item.None;

    public static void ClearEquippedItem()
    {
        for(int i=0; i<playerEquippedItem.Length; i++)
        {
            playerEquippedItem[i] = Item.None;
        }
        nextObtainItem = Item.None;
    }

    public static void SetNextEquippedItem()
    {
        nextObtainItem = (Item)UnityEngine.Random.Range(4, 13);

        // 하트가 2개 이상이면 다음 하트는 나오지 않도록 막는다
        while(equippedHeartCount >=2 && nextObtainItem == Item.Heart_UP)
        {
            nextObtainItem = (Item)UnityEngine.Random.Range(4, 13);
        }

        if(SceneManager.GetActiveScene().name == "Tutorial Rest 1")
        {
            nextObtainItem = Item.Heart_UP;
        }else if(SceneManager.GetActiveScene().name == "Tutorial Rest 2")
        {
            nextObtainItem = Item.Glass_Plus;
        }
    }

    public static void ObtainNextEquippedItem()
    {
        if(equippedItemCount == playerEquippedItem.Length) Debug.LogError("Count is Bigger than Box Size");

        playerEquippedItem[equippedItemCount] = nextObtainItem;

        nextObtainItem = Item.None;
    }

    public static int equippedItemCount{
        get{
            int i=0;
            for(; i<playerEquippedItem.Length; i++)
            {
                if(playerEquippedItem[i] == Item.None) break;
            }

            return i;
        }
    }

    public static int equippedHeartCount{
        get{
            int count = 0;
            for(int i=0; i<playerEquippedItem.Length; i++)
            {
                if(playerEquippedItem[i] == Item.None) break;
                if(playerEquippedItem[i] == Item.Heart_UP) count++;
            }

            return count;
        }
    }

    private static int StageBonusCalculator(Item item)
    {
        int amount = 0;
        int difficulty = (int)StageInformationManager.difficulty;

        foreach(Item equippedItem in playerEquippedItem)
        {
            if(item == Item.Potion && equippedItem == Item.Potion_Plus )
            {
                amount += StageInformationManager.plusPotion_byItem_perStage[difficulty];
            }else if(item == Item.Mag_Glass && equippedItem == Item.Glass_Plus )
            {
                amount += StageInformationManager.plusMag_byItem_perStage[difficulty];
            }else if(item == Item.Holy_Water && equippedItem == Item.Water_Plus )
            {
                amount += StageInformationManager.plusHoly_byItem_perStage[difficulty];
            }else if(Item.Time_Plus == item && Item.Time_Plus == equippedItem)
            {
                amount += StageInformationManager.plusTime_byItem_perStage[difficulty];
            }
        }

        return amount;
    }

    private static float percentageCalculator(Item item)
    {
        float percentage = 0;
        int difficulty = (int)StageInformationManager.difficulty;

        foreach(Item equippedItem in playerEquippedItem)
        {
            if(item == Item.Potion && equippedItem == Item.Potion_PercentageUP )
            {
                percentage += StageInformationManager.item_obtain_Up_Percentage[difficulty];
            }else if(item == Item.Mag_Glass && equippedItem == Item.Glass_PercentageUP )
            {
                percentage += StageInformationManager.item_obtain_Up_Percentage[difficulty];
            }else if(item == Item.Holy_Water && equippedItem == Item.Water_PercentageUP )
            {
                percentage += StageInformationManager.item_obtain_Up_Percentage[difficulty];
            }else if(Item.ALL_PercentageUP == item && Item.ALL_PercentageUP == equippedItem)
            {
                percentage += StageInformationManager.item_obtain_Up_Percentage[difficulty] / 2;
            }
        }

        return (percentage > 1) ? 1 : percentage;
    }

    private static int GetRandomValue(double probabilityOfOne)
    {
        if (probabilityOfOne < 0 || probabilityOfOne > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(probabilityOfOne), "Probability must be between 0.0 and 1.0");
        }

        System.Random _random = new System.Random();
        return _random.NextDouble() < probabilityOfOne ? 1 : 0;
    }

    public static int Time_StageBonus
    {
        get{
            return StageBonusCalculator(Item.Time_Plus);
        }
    }

    public static int Heart_StageBonus
    {
        get{
            return StageBonusCalculator(Item.Potion);
        }
    }

    public static int Glass_StageBonus
    {
        get{
            return StageBonusCalculator(Item.Mag_Glass);
        }
    }

    public static int Holy_StageBonus
    {
        get{
            return StageBonusCalculator(Item.Holy_Water);
        }
    }


    public static float AllPlus_percentage
    {
        get{
            return percentageCalculator(Item.ALL_PercentageUP);
        }
    }

    public static float HeartPlus_percentage
    {
        get{
            return percentageCalculator(Item.Potion);
        }
    }

    public static float GlassPlus_percentage
    {
        get{
            return percentageCalculator(Item.Mag_Glass);
        }
    }

    public static float HolyPlus_percentage
    {
        get{
            return percentageCalculator(Item.Holy_Water);
        }
    }

    public static int canObtainPlusItem(Item item)
    {
        if(item == Item.Potion)
        {
            return GetRandomValue(HeartPlus_percentage);
        }else if(item == Item.Mag_Glass)
        {
            return GetRandomValue(GlassPlus_percentage);
        }else if(item == Item.Holy_Water)
        {
            return GetRandomValue(HolyPlus_percentage);
        }else if(item == Item.ALL_PercentageUP)
        {
            return GetRandomValue(AllPlus_percentage);
        }

        return 0;
    }

}

public class LanguageManager{
    public static Action<string> languageChangeEvent;
    public static string currentLanguage;
    
    public static void Invoke_languageChangeEvent(string s)
    {
        currentLanguage = s;
        languageChangeEvent?.Invoke(s);
    }

    public static void LangaugeInitialize()
    {
        currentLanguage = PlayerPrefs.GetString("currentLanguage", "English");
        PlayerPrefs.SetString("currentLanguage",currentLanguage);
        PlayerPrefs.Save();
        Invoke_languageChangeEvent(currentLanguage);
    }

    public static void SaveLanguage(string s)
    {
        PlayerPrefs.SetString("currentLanguage",s);
        PlayerPrefs.Save();
    }
}

public class EventManager : MonoBehaviour
{
    #region Event

    public Action<EventType, Vector3Int> SetAnimationTileEvent;
    public Action<EventType, int> mine_treasure_count_Change_Event;
    public Action<EventType> Set_UI_Filter_Event;

    public Action<EventType,Item, int, int> Item_Count_Change_Event;
    public void Item_Count_Change_Invoke_Event(EventType eventType, Item item, int count, int changeAmount = 0 )
    {
        Item_Count_Change_Event.Invoke(eventType, item, count, changeAmount);
    }


    public Action<bool, GameOver_Reason> Game_Over_Event;

    public Action<int, int> Reduce_Heart_Event;
    public void Reduce_HeartInvokeEvent(int currentHeart, int maxHeart)
    {
        Reduce_Heart_Event.Invoke(currentHeart, maxHeart);
    }
    public Action<int, int, bool> Heal_Heart_Event;
    public void Heal_HeartInvokeEvent(int currentHeart, int maxHeart, bool isMaxUP = false)
    {
        Heal_Heart_Event.Invoke(currentHeart, maxHeart, isMaxUP);
    }


    public Action<int, int> timerEvent;
    public void TimerInvokeEvent(int timeElapsed, int timeLeft)
    {
        timerEvent.Invoke(timeElapsed, timeLeft);
    }

    public Action<Vector3Int,bool, bool, bool, bool, bool> ItemPanelShow_Event;
    public void ItemPanelShow_Invoke_Event(Vector3Int position, bool isShow, bool isHolyEnable = false, bool isCrachEnable = false, bool isMagEnable = false, bool isPotionEnable = false)
    {
        ItemPanelShow_Event.Invoke(position, isShow, isHolyEnable , isCrachEnable , isMagEnable , isPotionEnable);
    }

    public Action<ItemUseType, Vector3Int> ItemUseEvent;
    public void ItemUse_Invoke_Event(ItemUseType itemUseType, Vector3Int itemUseDirection)
    {
        ItemUseEvent.Invoke(itemUseType, itemUseDirection);
    }

    public Action StageClearEvent;
    public Action ObtainBigItemEvent;
    public void ObtainBigItem_Invoke_Event()
    {
        if(StageInformationManager.currentStageIndex < 5 && !(SceneManager.GetActiveScene().name == "Tutorial Last"))
        {
            ObtainBigItemEvent.Invoke();
            EquippedItem.ObtainNextEquippedItem();
            UpdateRightPanel_Invoke_Event();
        }else
        {
            StageClearEvent.Invoke();
        }
    }
    public void Invoke_StageClearEvent()
    {
        StageClearEvent.Invoke();
    }

    public Action UpdateRightPanelEvent;
    public void UpdateRightPanel_Invoke_Event()
    {
        UpdateRightPanelEvent.Invoke();
    }

    public Action UpdateLeftPanelEvent;
    public void UpdateLeftPanel_Invoke_Event()
    {
        UpdateLeftPanelEvent.Invoke();
    }

    public Action StairOpenEvent;
    public void StairOpen_Invoke_Event()
    {
        StairOpenEvent?.Invoke();
        GameAudioManager.instance.PlaySFXMusic(SFXAudioType.GateOpen);
    }

    public Action BackToMainMenuEvent;
    public void BackToMainMenu_Invoke_Event()
    {
        BackToMainMenuEvent?.Invoke();
        StageInformationManager.currentStageIndex = 0;
        EquippedItem.ClearEquippedItem();
    }

    public Action<string[] , bool , int , int > showNoticeUIEvent;
    public void Invoke_showNoticeUIEvent(string[] texts, bool isTyping, int panelWidth, int panelHeight)
    {
        showNoticeUIEvent?.Invoke(texts,isTyping, panelWidth, panelHeight);
    }

    public Action NoticeCountIncreaseEvent;
    public void Invoke_NoticeCountIncreaseEvent()
    {
        NoticeCountIncreaseEvent?.Invoke();
    }

    public Action TutorialTextTriggerEvent;
    public void Invoke_TutorialTextTriggerEvent()
    {
        TutorialTextTriggerEvent?.Invoke();
    }
    #endregion


    public static EventManager instance = null;
    public static bool isAnimationPlaying{
        get{
            return _AnimationPlaying;
        }

        set{
            _AnimationPlaying = value;
        }
    }

    static bool _AnimationPlaying = false;

    private void Awake() {
        if(instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }

    private void Start() {
        LanguageManager.LangaugeInitialize();
    }


    public void InvokeEvent(EventType eventType, System.Object param1 = null,System.Object param2 = null)
    {
        if(eventType == EventType.Game_Over)
        {
            if(param1 is GameOver_Reason)
            {
                Game_Over_Event.Invoke(true, (GameOver_Reason)param1);
            }
            
            return;
        }else if(eventType == EventType.Game_Restart)
        {
            Game_Over_Event.Invoke(false, GameOver_Reason.None);
            return;
        }

        if(param1 is int)
        {
            mine_treasure_count_Change_Event.Invoke(eventType, (int)param1);
            Set_UI_Filter_Event.Invoke(eventType);
        }

        if(param1 is Vector3Int)
        {
            SetAnimationTileEvent.Invoke(eventType, (Vector3Int)param1);
        }
        
    }


    
}
