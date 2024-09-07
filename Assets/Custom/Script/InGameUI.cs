using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour, AlertCallBack
{
    static public InGameUI instance;
    #region Serialize
    [Header("Texts")]
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI width;
    public TextMeshProUGUI height;
    public TextMeshProUGUI stageDifficulty;
    public TextMeshProUGUI stageType;
    public TextMeshProUGUI stageIndex;

    public TextMeshProUGUI trapDamage;
    public TextMeshProUGUI potionPlus;
    public TextMeshProUGUI magGlassPlus;
    public TextMeshProUGUI holyWaterPlus;
    public TextMeshProUGUI TimePlus;
    public TextMeshProUGUI left;
    public TextMeshProUGUI item;
    public TextMeshProUGUI stagePer;
    public TextMeshProUGUI[] buttonTexts;

    [Space]
    public TextMeshProUGUI[] usableItemExplain;
    public TextMeshProUGUI[] equipppedItemExplain;

    [Space]
    public TextMeshProUGUI leftTimeText;
    public TextMeshProUGUI elapsedTimeText;

    [Space]
    public TextMeshProUGUI mineCount;
    public TextMeshProUGUI treasureCount;

    [Space]
    public TextMeshProUGUI potionCount;
    public TextMeshProUGUI magGlassCount;
    public TextMeshProUGUI holyWaterCount;

    [Header("Heart Panel")]
    public Sprite heartFill;
    public Sprite heartEmpty;
    public Sprite heartNone;
    public Sprite[] heartReducingAnimationSprite;
    public RectTransform[] heartPanels; 

    [Space]
    [Header("Equipped Panel")]
    public Sprite[] EquippedItemSprites;
    public Image[] EquippedItemImages;


    [Space]
    [Header("Transforms")]
    float blackBoxMaxSize = 86f;
    public RectTransform SandClockTrans;
    public RectTransform upClockBlackBox;
    public RectTransform downClockBlackBox;

    [Space]
    public RectTransform totalItemPanel;
    public RectTransform[] ItemUses; // 0 : right, 1 : down, 2 : left, 3 : up
    private Button[] itemButtons; // 0 : right, 1 : down, 2 : left, 3 : up
    private Image[] itemimages; // 0 : right, 1 : down, 2 : left, 3 : up
    [Space]
    public Transform potionImage;
    public Transform magGlassImage;
    public Transform HolyWaterImage;


    [Space]
    [Header("Texts")]
    public string[] EnglishUsableText = {"Restores 1 unit of health", "Displays numbers on the ground to distinguish between traps and treasures", "When sprinkled on an obstacle, it reveals whether there's a treasure underneath or not"};
    public string[] KoreanUsableText = {"체력을 1칸 회복합니다", "바닥에 있는 숫자를 함정과 보물을 구분해서 보여줍니다", "장애물 아래에 보물이 있는지 없는지를 확인해 줍니다"};

    public string[] EnglishEquippedText = {
        "",
        "",
        "",
        "",
        "At the start of the stage, you receive an additional potion.",
        "At the start of the stage, you receive an additional magnifying glass.",
        "At the start of the stage, you receive an additional holy water.",
        "When obtaining a potion, the probability to get an additional potion increases.",
        "When obtaining a magnifying glass, the probability to get an additional magnifying glass increases.",  
        "When obtaining holy water, the probability to get an additional holy water increases.",
        "When obtaining an item, the probability to get an additional item of any kind increases.",
        "At the start of the stage, you receive additional time.",
        "Your maximum health increases by 3.",

    };

    public string[] KoreanEquippedText = {
        "",
        "",
        "",
        "",
        "스테이지 시작시, 포션을 추가로 지급받습니다",
        "스테이지 시작시, 돋보기를 추가로 지급받습니다",
        "스테이지 시작시, 성수를 추가로 지급받습니다",
        "포션 획득 시, 포션을 추가로 얻을 확률이 올라갑니다",
        "돋보기 획득 시, 돋보기를 추가로 얻을 확률이 올라갑니다",
        "성수 획득 시, 성수를 추가로 얻을 확률이 올라갑니다",
        "아이템 획득 시, 모든 아이템이 추가로 얻을 확률이 올라갑니다",
        "스테이지 시작시, 제한시간을 추가로 지급받습니다",
        "최대 체력이 3 올라갑니다",
    };

    #endregion

    private Image[] heartImages = new Image[9];
    private InGameUIAniimation inGameUIAniimation;

    public delegate void AlertCallBackDelgate();

    public void GameRestart()
    {
        EventManager.instance.InvokeEvent(EventType.Game_Restart, GameOver_Reason.Heart0);
    }

    private AlertCallBackDelgate callbackFunction;

    public void GameRestartAlertShow()
    {
        callbackFunction = GameRestartMenu;
        AlertUI.instance.ShowAlert(AlertUI.AlertSituation.Restart, this);
    }

    public void GameMainMenuAlertShow()
    {
        callbackFunction = GoBackMainMenu;
        AlertUI.instance.ShowAlert(AlertUI.AlertSituation.GoBackMainMenu, this);
    }

    public void GameRestartMenu()
    {
        MainMenu.RestartManageClass.restartGameModeType = StageInformationManager.getGameMode();
        GoBackMainMenu();
    }

    public void GoBackMainMenu()
    {
        MakeScreenBlack.Hide();
        EventManager.instance.BackToMainMenu_Invoke_Event();

        StageManager.stageInputBlock =0;
        
        LoadingInformation.loadingSceneName = "Main Menu";
        SceneManager.LoadScene("Loading");
    }

    public void ItemUse(int numtype)
    {
        EventManager.instance.ItemUse_Invoke_Event((ItemUseType)numtype, StageManager.instance.gapBetweenPlayerFocus);
        EventManager.instance.ItemPanelShow_Invoke_Event(Vector3Int.zero, false);
        StageManager.isNowInputtingItem = false;
    }

    private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }
        
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                heartImages[i*3 + j] = heartPanels[i].GetChild(j).GetComponent<Image>();
                heartImages[i*3 + j].sprite = heartNone;
            }
        }

        inGameUIAniimation = GetComponent<InGameUIAniimation>();

        itemButtons = totalItemPanel.GetComponentsInChildren<Button>();
        itemimages = totalItemPanel.GetComponentsInChildren<Image>();

        for(int i=0; i<equipppedItemExplain.Length; i++)
            {
                equipppedItemExplain[i].text = "";
            }

        usableItemExplain[0].text = KoreanUsableText[0];
        usableItemExplain[1].text = KoreanUsableText[1];
        usableItemExplain[2].text = KoreanUsableText[2];
    }

    private void SetLanguageTexts()
    {
        for(int i=0; i<equipppedItemExplain.Length; i++)
        {
            equipppedItemExplain[i].text = "";
        }

        if(LanguageManager.currentLanguage == "English")
        {
            usableItemExplain[0].text = EnglishUsableText[0];
            usableItemExplain[1].text = EnglishUsableText[1];
            usableItemExplain[2].text = EnglishUsableText[2];

        }else if(LanguageManager.currentLanguage == "Korean")
        {
            usableItemExplain[0].text = KoreanUsableText[0];
            usableItemExplain[1].text = KoreanUsableText[1];
            usableItemExplain[2].text = KoreanUsableText[2];
        }
    }


    private void OnEnable() {
        EventManager.instance.mine_treasure_count_Change_Event += Change_Mine_Treasure_Count;
        EventManager.instance.Reduce_Heart_Event += Reduce_Heart;
        EventManager.instance.Heal_Heart_Event += Heal_Heart;

        EventManager.instance.Item_Count_Change_Event += Change_Item_Count;
        EventManager.instance.ItemPanelShow_Event += ShowItemUsePanel;

        EventManager.instance.timerEvent += SetTimeTexts;

        EventManager.instance.UpdateRightPanelEvent += UpdateRightPanel;
        EventManager.instance.UpdateLeftPanelEvent += UpdateLeftPanel;

        EventManager.instance.BackToMainMenuEvent += DestroyUI;

        LanguageManager.languageChangeEvent += UpdateLeftPanelS;
        LanguageManager.languageChangeEvent += UpdateRightPanelS;
    }

    private void OnDisable() {
        EventManager.instance.mine_treasure_count_Change_Event -= Change_Mine_Treasure_Count;
        EventManager.instance.Reduce_Heart_Event -= Reduce_Heart;
        EventManager.instance.Heal_Heart_Event -= Heal_Heart;

        EventManager.instance.Item_Count_Change_Event -= Change_Item_Count;
        EventManager.instance.ItemPanelShow_Event -= ShowItemUsePanel;

        EventManager.instance.timerEvent -= SetTimeTexts;
        EventManager.instance.UpdateRightPanelEvent -= UpdateRightPanel;
        EventManager.instance.UpdateLeftPanelEvent -= UpdateLeftPanel;

        EventManager.instance.BackToMainMenuEvent -= DestroyUI;
        LanguageManager.languageChangeEvent -= UpdateLeftPanelS;
        LanguageManager.languageChangeEvent -= UpdateRightPanelS;
    }

    public void DestroyUI()
    {
        Destroy(this.gameObject);
    }

    public Vector2 WorldToCanvasPosition(Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        screenPosition -= new Vector3(Screen.width /2 , Screen.height/2 , 0);

        return screenPosition;
    }


    private void ShowItemUsePanel(Vector3Int position, bool isShow, bool isHolyEnable = false, bool isCrachEnable = false, bool isMagEnable = false, bool isPotionEnable = false)
    {
        Vector2 screenPoint = WorldToCanvasPosition(TileGrid.CheckWorldPosition(position));

        if(isShow)
        {
            if(isHolyEnable)
            {
                itemButtons[0].interactable = true;
            }else
            {
                itemButtons[0].interactable = false;
            }

            if(isCrachEnable)
            {
                itemButtons[1].interactable = true;
            }else
            {
                itemButtons[1].interactable = false;
            }

            if(isMagEnable)
            {
                itemButtons[2].interactable = true;
            }else
            {
                itemButtons[2].interactable = false;
            }

            if(isPotionEnable)
            {
                itemButtons[3].interactable = true;
            }else
            {
                itemButtons[3].interactable = false;
            }

            totalItemPanel.anchoredPosition = screenPoint;

            inGameUIAniimation.Set_Item_Use(true);
            

        }else
        {
            inGameUIAniimation.Set_Item_Use(false);
        }
    }

    private void SetTimeTexts(int elapsedTime, int leftTime)
    {
        elapsedTimeText.text = elapsedTime.ToString();
        leftTimeText.text = leftTime.ToString();

        if(elapsedTime == 0 && leftTime == 0) return;
        
        float percentageToChangeBlackBox = (float)elapsedTime / (elapsedTime + leftTime);
        upClockBlackBox.sizeDelta = new Vector2(upClockBlackBox.sizeDelta.x, percentageToChangeBlackBox * blackBoxMaxSize);
        downClockBlackBox.sizeDelta = new Vector2(downClockBlackBox.sizeDelta.x, (1 - percentageToChangeBlackBox) * blackBoxMaxSize);

        float changeDelta = percentageToChangeBlackBox * percentageToChangeBlackBox / 20;

        StartCoroutine(changeClockSizeShortly(changeDelta * 0.8f + 0.005f));
    }

    IEnumerator changeClockSizeShortly(float P)
    {
        float deltaP = P / 3;

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x - deltaP, SandClockTrans.localScale.y - deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x - deltaP, SandClockTrans.localScale.y - deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x - deltaP, SandClockTrans.localScale.y - deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x + deltaP, SandClockTrans.localScale.y + deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x + deltaP, SandClockTrans.localScale.y + deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x + deltaP, SandClockTrans.localScale.y + deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x + deltaP, SandClockTrans.localScale.y + deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x + deltaP, SandClockTrans.localScale.y + deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x + deltaP, SandClockTrans.localScale.y + deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x - deltaP, SandClockTrans.localScale.y - deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x - deltaP, SandClockTrans.localScale.y - deltaP,0);

        yield return new WaitForSeconds(0.04f);
        SandClockTrans.localScale = new Vector3(SandClockTrans.localScale.x - deltaP, SandClockTrans.localScale.y - deltaP,0);
    }

    private void Change_Item_Count(EventType eventType, Item usableItem , int count, int changeAmount)
    {
        bool flag = false;

        if(usableItem == Item.None)
        {
            inGameUIAniimation.SetItem_Use_Obtain_Flag(usableItem, changeAmount);
            return;
        }

        if(eventType == EventType.Item_Obtain && changeAmount > 0)
        {
            inGameUIAniimation.SetItem_Use_Obtain_Flag(usableItem, changeAmount);
        }
        
        
        if(eventType == EventType.Item_Use)
        {
            flag = false;
        }else if(eventType == EventType.Item_Obtain)
        {
            flag = true;
        }

        switch(usableItem)
        {
            case Item.Potion :
                potionCount.text = ": " + count.ToString();
                if(StageManager.isNowInitializing) return;
                StartCoroutine(changeItemSize(potionImage ,potionCount , flag));
                break;
            case Item.Mag_Glass :
                magGlassCount.text = ": " + count.ToString();
                if(StageManager.isNowInitializing) return;
                StartCoroutine(changeItemSize(magGlassImage , magGlassCount , flag));
                break;
            case Item.Holy_Water :
                holyWaterCount.text = ": " + count.ToString();
                if(StageManager.isNowInitializing) return;
                StartCoroutine(changeItemSize(HolyWaterImage , holyWaterCount , flag));
                break;
        }
    }

    IEnumerator changeItemSize(Transform imageTransform, TextMeshProUGUI count, bool goBigger)
    {
        float changeSizeUnit = 0.03f;
        float C = (goBigger) ? 1 : -1;

        if(goBigger)
        {
            count.color = Color.green;
        }else
        {
            count.color = Color.red;
        }

        imageTransform.localScale = new Vector3(imageTransform.localScale.x + C * changeSizeUnit, imageTransform.localScale.y +  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x + C * changeSizeUnit, imageTransform.localScale.y +  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x + C * changeSizeUnit, imageTransform.localScale.y +  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x + C * changeSizeUnit, imageTransform.localScale.y +  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x + C * changeSizeUnit, imageTransform.localScale.y +  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x - C * changeSizeUnit, imageTransform.localScale.y -  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x - C * changeSizeUnit, imageTransform.localScale.y -  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x - C * changeSizeUnit, imageTransform.localScale.y -  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x - C * changeSizeUnit, imageTransform.localScale.y -  C *changeSizeUnit,0);
        yield return new WaitForSeconds(0.02f);

        imageTransform.localScale = new Vector3(imageTransform.localScale.x - C * changeSizeUnit, imageTransform.localScale.y -  C *changeSizeUnit,0);


        count.color = Color.white;
    }


    private void Change_Mine_Treasure_Count(EventType eventType, int count)
    {
        switch(eventType)
        {
            case EventType.MineAppear :
                mineCount.text = count.ToString();
                StartCoroutine(changeTextColorShortly(mineCount, Color.red, Color.white));
                break;
            case EventType.MineDisappear :
                mineCount.text = count.ToString();
                StartCoroutine(changeTextColorShortly(mineCount, Color.red, Color.white));
                break;
            case EventType.TreasureAppear :
                treasureCount.text = count.ToString();
                StartCoroutine(changeTextColorShortly(treasureCount, Color.yellow, Color.white));
                break;
            case EventType.TreasureDisappear :
                treasureCount.text = count.ToString();
                StartCoroutine(changeTextColorShortly(treasureCount, Color.yellow, Color.white));
                break;
            case EventType.None :
                treasureCount.text = count.ToString();
                mineCount.text = count.ToString();
                StartCoroutine(changeTextColorShortly(treasureCount, Color.yellow, Color.white));
                StartCoroutine(changeTextColorShortly(mineCount, Color.red, Color.white));
                break;
        }
    }

    private void UpdateRightPanelS(string s)
    {
        UpdateRightPanel();
    }
    private void UpdateRightPanel()
    {
        string[] EnglisButton = {"Minimap", "Menu", "Restart", "Tutorial", "Setting", "Main Menu"};
        string[] KoreanButton = {"미니맵", "메뉴", "재시작", "튜토리얼", "환경설정", "메인메뉴"};
        string[] tempButtonText = EnglisButton;
        for(int i=0; i<5; i++)
        {
            EquippedItemImages[i].sprite = EquippedItemSprites[0];
            equipppedItemExplain[i].text = "";
        }

        if(LanguageManager.currentLanguage == "Korean")
        {
            item.text = "아이템";
            tempButtonText = KoreanButton;
        }else if(LanguageManager.currentLanguage == "English")
        {
            item.text = "Item";
            tempButtonText = EnglisButton;
        }

        SetLanguageTexts();

        for(int i=0; i<buttonTexts.Length; i++)
        {
            buttonTexts[i].text = tempButtonText[i];
        }

    }

    private void UpdateLeftPanelS(string s)
    {
        UpdateLeftPanel();
    }



    private void UpdateLeftPanel()
    {
        string[] EnglishDifficulty = {"Easy", "Normal", "Hard"};
        string[] KoreanDifficulty = {"쉬움", "보통", "어려움"};

        string[] EnglishStage = {"Cave", "Crypt", "Ruin"};
        string[] KoreanStage = {"동굴", "묘지", "폐허"};

        int difficulty = (int)StageInformationManager.difficulty;
        int stagetype = StageInformationManager.currentStagetype;


        if(LanguageManager.currentLanguage == "Korean")
        {
            difficultyText.text = KoreanDifficulty[difficulty];
            stageDifficulty.text = KoreanDifficulty[difficulty];
            stageType.text = KoreanStage[stagetype];
            width.text = "너비 : " + StageInformationManager.NextWidth.ToString();
            height.text = "높이 : " + StageInformationManager.NextHeight.ToString();
            stageIndex.text = "스테이지";

            
            left.text = "남은 수";
            stagePer.text = "스테이지 마다";
        }else if(LanguageManager.currentLanguage == "English")
        {
            difficultyText.text = EnglishDifficulty[difficulty];
            stageDifficulty.text = EnglishDifficulty[difficulty];
            stageType.text = EnglishStage[stagetype];
            width.text = "Width : " + StageInformationManager.NextWidth.ToString();
            height.text = "Height : " + StageInformationManager.NextHeight.ToString();
            stageIndex.text = "STAGE";

            
            left.text = "Left";
            stagePer.text = "stage per";
        }
        
        trapDamage.text = "X" + StageInformationManager.DefaultTrapDamage[difficulty].ToString();
        
        potionPlus.text = "+" +0.ToString();
        magGlassPlus.text = "+" + 0.ToString();
        holyWaterPlus.text = "+" + 0.ToString();
        TimePlus.text = "+" +  0.ToString();

        
        SetLanguageTexts();
    }

    IEnumerator changeTextColorShortly(TextMeshProUGUI textMeshProUGUI, Color standardColor, Color changeColor)
    {
        int changeSizeUnit = 5;

        textMeshProUGUI.color = changeColor;
        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize - changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize - changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize - changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize - changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize + changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize + changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize + changeSizeUnit;

        yield return new WaitForSeconds(0.02f);

        textMeshProUGUI.fontSize = textMeshProUGUI.fontSize + changeSizeUnit;
        textMeshProUGUI.color = standardColor;
    }

    private void Reduce_Heart(int currentHeart, int maxHeart)
    {
        for(int i=0; i<heartImages.Length; i++)
        {
            heartImages[i].gameObject.SetActive(false);
        }

        for(int i=0; i<maxHeart; i++)
        {
            heartImages[i].gameObject.SetActive(true);
            heartImages[i].sprite = heartEmpty;
        }

        for(int i=0; i<currentHeart; i++)
        {
            heartImages[i].sprite = heartFill;
        }

        if(StageManager.isNowInitializing) return;

        StartCoroutine(heartReducing(currentHeart));
    }

    private void Heal_Heart(int currentHeart, int maxHeart, bool isMaxUP)
    {
        for(int i=0; i<heartImages.Length; i++)
        {
            heartImages[i].gameObject.SetActive(false);
        }

        for(int i=0; i<maxHeart; i++)
        {
            heartImages[i].gameObject.SetActive(true);
            heartImages[i].sprite = heartEmpty;
        }

        if(isMaxUP)
        {
            for(int i=0; i<currentHeart-3; i++)
            {
                heartImages[i].sprite = heartFill;
            }

        }else
        {
            for(int i=0; i<currentHeart-1; i++)
            {
                heartImages[i].sprite = heartFill;
            }
        }

        

        if(StageManager.isNowInitializing) return;

        if(isMaxUP)
        {
            StartCoroutine(hearMaxUP(currentHeart-1));
        }else
        {
            StartCoroutine(heartHealing(currentHeart-1));
        }
        
    }

    IEnumerator hearMaxUP(int index)
    {
        yield return StartCoroutine(heartHealing(index-2));
        yield return StartCoroutine(heartHealing(index-1));
        yield return StartCoroutine(heartHealing(index));
    }

    IEnumerator heartReducing(int index)
    {
        float changeUnit = 0.1f;
        for(int i=0; i< 5; i++)
        {
            heartImages[index].gameObject.transform.localScale = new Vector3(1 + i * changeUnit,1 + i * changeUnit,1);
            yield return new WaitForSeconds(0.02f);
        }
        
        foreach(Sprite sprite in heartReducingAnimationSprite)
        {
            heartImages[index].sprite = sprite;
            yield return new WaitForSeconds(0.05f);
        }

        for(int i=0; i< 5; i++)
        {
            heartImages[index].gameObject.transform.localScale = new Vector3(1 + (4 - i)* changeUnit,1 + (4 - i) * changeUnit,1);
            yield return new WaitForSeconds(0.02f);
        }

        heartImages[index].gameObject.transform.localScale = Vector3.one;
    }

    IEnumerator heartHealing(int index)
    {
        float changeUnit = 0.1f;
        for(int i=0; i< 5; i++)
        {
            heartImages[index].gameObject.transform.localScale = new Vector3(1 + i * changeUnit,1 + i * changeUnit,1);
            yield return new WaitForSeconds(0.02f);
        }
        
        foreach(Sprite sprite in heartReducingAnimationSprite.Reverse().ToArray())
        {
            heartImages[index].sprite = sprite;
            yield return new WaitForSeconds(0.05f);
        }

        for(int i=0; i< 5; i++)
        {
            heartImages[index].gameObject.transform.localScale = new Vector3(1 + (4 - i)* changeUnit,1 + (4 - i) * changeUnit,1);
            yield return new WaitForSeconds(0.02f);
        }

        heartImages[index].gameObject.transform.localScale = Vector3.one;
    }

    public void CallBack()
    {
        callbackFunction();
    }
}
