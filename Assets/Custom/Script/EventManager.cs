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

    public Action TutorialShowEvent;

    public Action<Vector3> SetFocusEvent;

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

    public Action AfterMoveCallBackEvent;

    public Action StageClearEvent;
    public Action ObtainBigItemEvent;

    public void Invoke_StageClearEvent()
    {
        StageClearEvent.Invoke();
    }

    public Action UpdateRightPanelEvent;
    public void UpdateRightPanel_Invoke_Event()
    {
        UpdateRightPanelEvent?.Invoke();
    }

    public Action UpdateLeftPanelEvent;
    public void UpdateLeftPanel_Invoke_Event()
    {
        UpdateLeftPanelEvent?.Invoke();
    }

    public Action BackToMainMenuEvent;
    public void BackToMainMenu_Invoke_Event()
    {
        BackToMainMenuEvent?.Invoke();
        StageInformationManager.currentStageIndex = 0;
    }

    public Action NoticeCountIncreaseEvent;
    public void Invoke_NoticeCountIncreaseEvent()
    {
        NoticeCountIncreaseEvent?.Invoke();
    }

    #endregion

    public Action<string[] , bool , int , int > showNoticeUIEvent;
    public void Invoke_showNoticeUIEvent(string[] texts, bool isTyping, int panelWidth, int panelHeight)
    {
        showNoticeUIEvent?.Invoke(texts,isTyping, panelWidth, panelHeight);
    }


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
