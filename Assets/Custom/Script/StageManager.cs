using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;



public class StageManager : MonoBehaviour, IStageManager
{   
    public bool isTutorial = false;
    public int tutorialStage;

    public static StageManager instance;
    public static bool isDungeon = true;
    const int DefaultX = 18;
    const int DefaultY = 12;

    static public bool isNowInitializing = false;

    /// <summary>
    /// 스테이지에 입력을 받을지 말지 정한다. 이게 0이면 스테이지 인풋을 받고, 아니면 차단
    /// </summary>
    static public int stageInputBlock{
        get{
            return _stageInputBlock;
        }

        set{
            _stageInputBlock =  value;
            if(_stageInputBlock < 0) _stageInputBlock = 0;
        }
    }

    static public bool isStageInputBlocked{
        get{
            return (stageInputBlock > 0 || EventManager.isAnimationPlaying);
        }

    }

    static private int _stageInputBlock = 0; 

    [SerializeField] private TileGrid grid;

    [Space]
    [Header("For Debug")]

    Vector3Int BigTreasurePosition;

    int startX = -1;
    int startY = -1;

    int width = -1;
    int height = -1;

    public int maxHeart{
        get{
            return _maxHeart;
        }

        set{
            _maxHeart = value;
        }
    }

    public int currentHeart{
        get{
            return _currentHeart;
        }

        set{
            _currentHeart = value;
        }
    }

    public int mineCount{
        get{
            return _mineCount;
        }
        set{
            _mineCount = value;
        }
    }
    public int treasureCount{
        get{
            return _treasureCount;
        }
        set{
            _treasureCount = value;
            if(_treasureCount == 0)
            {
                if(StageInformationManager.getGameMode() == GameModeType.stage)
                {
                    // 스테이지 모드에서 모든 보물을 먹으면 거기서 게임 클리어
                    EventManager.instance.Invoke_StageClearEvent();
                }else
                {
                    EventManager.instance.StairOpen_Invoke_Event();
                    if(isTutorial && isDungeon && (tutorialStage == 1 && TutorialGuide.tutorialTextindex == 4)  || (tutorialStage == 2 && TutorialGuide.tutorialTextindex == 2)|| 
                    (tutorialStage == 3 && TutorialGuide.tutorialTextindex == 2)||  (tutorialStage == 4 && TutorialGuide.tutorialTextindex == 1) )
                    {
                        EventManager.instance.Invoke_TutorialTextTriggerEvent();
                    }  
                }
                    
            }
        }
    }

    private int _maxHeart = 0;
    private int _currentHeart = 0;
    private int _mineCount = 0;
    private int _treasureCount = 0;

    int[,] mineTreasureArray; // -2 : treausre, -1 : mine, 1 : Start Safe Area

    int[,] totalNumArray = null;
    bool[,] totalNumMask = null;

    bool[,] treasureSearchMask = null;

    int[,] mineNumArray = null;
    int[,] treasureNumArray = null;

    int[,] flagArray = null;

    bool[,] isObstacleRemoved = null;


    #region ITEM_Field
    int potionCount = 0;
    int magGlassCount = 0;
    int holyWaterCount = 0;

    bool isFoucusOnObstacle {
        get{
            return TileGrid.CheckObstaclePosition(currentFocusPosition);
        }
    }

    bool potionEnable{
        get{
            return potionCount > 0 && !isFoucusOnObstacle && (currentHeart != maxHeart);
        }
    }

    bool magGlassEnable{
        get{
            return magGlassCount > 0 && !isFoucusOnObstacle;
        }
    }

    bool holyWaterEnable{
        get{
            return holyWaterCount > 0 && isFoucusOnObstacle && !(isTutorial && tutorialStage == 2  && isDungeon);
        }
    }
    #endregion

    private Vector3Int currentFocusPosition = Vector3Int.one;

    private int totalTime = 0;
    private int timeElapsed = 0;
    private int timeLeft {
        get{
            return totalTime - timeElapsed;
        }
    }
    private Coroutine timerCoroutine = null;

    public delegate bool ConditionDelegate(int x);
    List<ConditionDelegate> NumModeConditions = new List<ConditionDelegate>
        {
            (x) => x < 0 ,  // 토탈로 보면 0보다 작은 경우는 전부 센다
            (x) => x == -1,  // 지뢰인 경우를 찾는다
            (x) => x == -2  // 보물인 경우를 찾는다
        };
    private int[] aroundX = {-1,0,1 };
    private int[] aroundY = {-1,0,1 };

    static public bool isNowInputtingItem = false;

    public Vector3Int gapBetweenPlayerFocus{
        get{
            return PlayerManager.instance.checkPlayerNearFourDirection(currentFocusPosition);
        }
    } 

    public bool isNearFlag{
        get{
            // 상하좌우 4개 근처인지를 판단
            return ((gapBetweenPlayerFocus.magnitude == 1 || gapBetweenPlayerFocus.magnitude == 0) && gapBetweenPlayerFocus != Vector3Int.forward) ? true : false; 
        }
    }

    public bool interactOkflag{
        get{
            return (gapBetweenPlayerFocus == Vector3Int.zero) || (isNearFlag && isFoucusOnObstacle); 
        }
    }
             
         
    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Debug.LogError("Youre now trying to reInstantiate StageManager while there is Original StageManager");
        }

    }

    private void OnDestroy() {
        instance = null;

        if(LoadingInformation.loadingSceneName == "Main Menu") 
        {
            EquippedItem.ClearEquippedItem();
            return;
        }
        

        if(!StageInformationManager.isnextStageDungeon) // 다음이 던전인 경우
        {
            StageInformationManager.currentStageIndex++;
            int min = StageInformationManager.stageWidthMin[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
            int max = StageInformationManager.stageWidthMax[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
            
            if(isTutorial)
                {
                    StageInformationManager.NextWidth = StageInformationManager.tutorialWidth[StageInformationManager.currentStageIndex];
                    StageInformationManager.NextHeight= StageInformationManager.tutorialHeight[StageInformationManager.currentStageIndex];
                }else
                {
                    StageInformationManager.NextWidth = UnityEngine.Random.Range(min, max+1); 
                    StageInformationManager.NextHeight = StageInformationManager.stageHeightMin[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
                }
            
        }
        StageInformationManager.setHearts(maxHeart, currentHeart); 
        StageInformationManager.setUsableItems(potionCount,magGlassCount,holyWaterCount);
        StageInformationManager. NexttotalTime = timeLeft;

        StageInformationManager.isnextStageDungeon = !StageInformationManager.isnextStageDungeon;
    }

    private void Start() {
        MakeScreenBlack.Clear();
        if(StageInformationManager.getGameMode() == GameModeType.stage)
        {
            int difficulty = (int)StageInformationManager.difficulty;        
            int stageType = StageInformationManager.currentStagetype;

            StageInformationManager.NextWidth = StageInformationManager.StageModestageWidth[stageType];
            StageInformationManager.NextHeight= StageInformationManager.StageModestageHeight[stageType];
            StageInformationManager.setHearts(3,3); 
            StageInformationManager.setUsableItems(0,StageInformationManager.StageMagItemAmount[stageType,difficulty] , 0);
            StageInformationManager.NexttotalTime = StageInformationManager.StageModeTime[stageType,difficulty];

            int[] usableItems = StageInformationManager.getUsableItems();
            int[] hearts = StageInformationManager.getHearts();

            DungeonInitialize(StageInformationManager.NextWidth, StageInformationManager.NextHeight ,
            StageInformationManager.difficulty ,hearts[0], hearts[1], 
            usableItems[0], usableItems[1], usableItems[2], 
            StageInformationManager. NexttotalTime);

            return;
        }

        if(StageInformationManager.isnextStageDungeon)
        {
            if(StageInformationManager.currentStageIndex == 0)
            {
                if(isTutorial)
                {
                    StageInformationManager.NextWidth = StageInformationManager.tutorialWidth[0];
                    StageInformationManager.NextHeight= StageInformationManager.tutorialHeight[0];

                    StageInformationManager.difficulty = Difficulty.Easy;
                    StageInformationManager.setHearts(); 
                    StageInformationManager.setUsableItems(0,0,0);
                    StageInformationManager. NexttotalTime = StageInformationManager.DefaultTimeforEntireGame;

                }else
                {
                    StageInformationManager.setHearts(); 
                    StageInformationManager.setUsableItems();
                    StageInformationManager. NexttotalTime = StageInformationManager.DefaultTimeforEntireGame;
                }
            }

            int[] usableItems = StageInformationManager.getUsableItems();
            int[] hearts = StageInformationManager.getHearts();
            DungeonInitialize(StageInformationManager.NextWidth, StageInformationManager.NextHeight ,StageInformationManager.difficulty ,hearts[0], hearts[1], usableItems[0], usableItems[1], usableItems[2], StageInformationManager. NexttotalTime);
            
            
        }else
        {
            int[] usableItems = StageInformationManager.getUsableItems();
            int[] hearts = StageInformationManager.getHearts();
            RestPlaceInitialize(StageInformationManager.treasurePosition, hearts[0], hearts[1], usableItems[0], usableItems[1], usableItems[2], StageInformationManager. NexttotalTime);
        }
        
    }

    public void ItemPanelShow(bool flag)
    {
        if(flag)
        {
            if(!isNowInputtingItem)
            {
                if(!interactOkflag) return;

                

                isNowInputtingItem = true;
                EventManager.instance.ItemPanelShow_Invoke_Event(currentFocusPosition, true, holyWaterEnable, isFoucusOnObstacle && !(isTutorial && tutorialStage == 2  && isDungeon), magGlassEnable , potionEnable);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.itemMenuShow);  

                if(isTutorial && tutorialStage == 2  && isDungeon && !isFoucusOnObstacle && TutorialGuide.tutorialTextindex == 1)
                {
                    EventManager.instance.Invoke_TutorialTextTriggerEvent();
                }  

                if(isTutorial && tutorialStage == 3  && isDungeon && isFoucusOnObstacle && TutorialGuide.tutorialTextindex == 1)
                {
                    EventManager.instance.Invoke_TutorialTextTriggerEvent();
                }  

            }
        }else
        {
            if(isNowInputtingItem)
            {
                isNowInputtingItem = false;
                EventManager.instance.ItemPanelShow_Invoke_Event(currentFocusPosition, false);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.itemMenuClose);
            }
        }
    }

    public void MoveOrShovelOrInteract(bool shovelLock = false)
    {
        if(isFoucusOnObstacle)
        {
            if(!isNearFlag) return;
            if(isNowInputtingItem) return;
            if(shovelLock) return;

            if(isDungeon)
            {
                EventManager.instance.ItemUse_Invoke_Event(ItemUseType.Shovel, gapBetweenPlayerFocus);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Shovel);
                RemoveObstacle(currentFocusPosition);     

                if(isTutorial && tutorialStage == 1 &&isDungeon && TutorialGuide.tutorialTextindex == 3)
                {
                    EventManager.instance.Invoke_TutorialTextTriggerEvent();
                }       
            }else
            {
                if(BigTreasurePosition == currentFocusPosition) 
                {
                    BigTreasurePosition = Vector3Int.forward;
                    EquippedItem.SetNextEquippedItem();
                    if(EquippedItem.nextObtainItem == Item.Heart_UP) MaxHeartUP();
                    EventManager.instance.ObtainBigItem_Invoke_Event();
                    GameAudioManager.instance.PlaySFXMusic(SFXAudioType.GetBigItem);

                    if(isTutorial && (tutorialStage == 1 || tutorialStage == 2 || tutorialStage == 3) && !isDungeon && TutorialGuide.tutorialTextindex == 1)
                    {
                        EventManager.instance.Invoke_TutorialTextTriggerEvent();
                    } 
                }
                
            }

        }else
        {
            if(isNowInputtingItem) return;

            if(isDungeon)
            {
                if(hasTrapInPosition(currentFocusPosition)) return;
            }
            
            InputManager.InputEvent.Invoke_Move(currentFocusPosition);


            if(isTutorial && tutorialStage == 1 && isDungeon && TutorialGuide.tutorialTextindex == 1)
            {
                EventManager.instance.Invoke_TutorialTextTriggerEvent();
            }
        }
    }

    private void Update() {

        if(EventSystem.current.IsPointerOverGameObject()) return;
        if(isNowInitializing) return;

        SetFocus();

        if(isDungeon)
        {
            SetPlayer_Overlay();
            SetInteract_Ok();
        }
        
        
    }

    private void OnEnable() {
        EventManager.instance.Game_Over_Event += GameOver;
        EventManager.instance.ItemUseEvent += ItemUse;
        EventManager.instance.UpdateRightPanel_Invoke_Event();
    }

    private void OnDisable() {
        EventManager.instance.Game_Over_Event -= GameOver;
        EventManager.instance.ItemUseEvent -= ItemUse;
    }

    private Vector3Int[] InteractPosition1 = new Vector3Int[5]{Vector3Int.zero,Vector3Int.forward, Vector3Int.forward,Vector3Int.forward,Vector3Int.forward};
    private Vector3Int[] InteractPosition2 = new Vector3Int[5]{Vector3Int.zero,Vector3Int.forward, Vector3Int.forward,Vector3Int.forward,Vector3Int.forward};
    private bool is1Next = true;
    private Vector3Int[] iterateMap = new Vector3Int[5]{Vector3Int.zero,Vector3Int.up, Vector3Int.down, Vector3Int.right, Vector3Int.left};

    private void ItemUse(ItemUseType itemUseType, Vector3Int gap)
    {
        switch(itemUseType)
        {
            case ItemUseType.Holy_Water :
                SetTreasureSearch(currentFocusPosition);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.HolyWater);
            break;
            case ItemUseType.Crash :
                BombObstacle(currentFocusPosition);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Bomb);
            break;
            case ItemUseType.Mag_Glass :
                ChangeTotalToSeperate(currentFocusPosition);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Mag_Glass);
            break;
            case ItemUseType.Potion :
                potionCount--;
                EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Use, Item.Potion, potionCount);
                HeartChange(1);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.potion);
            break;
        }
    }
    
    private void SetInteract_Ok()
    {
       Vector3Int playerPosition = PlayerManager.instance.PlayerCellPosition;

       if(is1Next)
       {
            InteractPosition1[0] = playerPosition + iterateMap[0];

            for(int i=1; i<5; i++)
            {
                if(grid.obstacleTilemap.HasTile(playerPosition + iterateMap[i]))
                {
                    InteractPosition1[i] = playerPosition + iterateMap[i];
                }else
                {
                    InteractPosition1[i] = Vector3Int.forward;
                }
            }

            grid.SetInteract_Ok(InteractPosition2,InteractPosition1);
       }else
       {
            InteractPosition2[0] = playerPosition + iterateMap[0];

            for(int i=1; i<5; i++)  
            {
                if(grid.obstacleTilemap.HasTile(playerPosition + iterateMap[i]))
                {
                    InteractPosition2[i] = playerPosition + iterateMap[i];
                }else
                {
                    InteractPosition2[i] = Vector3Int.forward;
                }
            }

            grid.SetInteract_Ok(InteractPosition1,InteractPosition2);
       }
       

       is1Next = !is1Next;

    }

    private Vector3Int currentPlayerPosition = Vector3Int.zero;
    private void SetPlayer_Overlay(bool isForce = false)
    {
        Vector3Int playerPosition = PlayerManager.instance.PlayerCellPosition;
        Vector3Int arrayPos = ChangeCellPosToArrayPos(playerPosition);

        if(currentPlayerPosition == playerPosition && !isForce) return; // 만약 플레이어 위치가 변하지 않았다면 그냥 아무것도 안함

        grid.ShowOverlayNum(currentPlayerPosition,false, true);

        currentPlayerPosition = playerPosition;

        if(totalNumMask[arrayPos.y, arrayPos.x]) // 만약 돋보기를 쓴 경우
        {
            grid.ShowOverlayNum(playerPosition,false,true);
            grid.ShowOverlayNum(playerPosition,true,false,mineNumArray[arrayPos.y, arrayPos.x], treasureNumArray[arrayPos.y, arrayPos.x]);
            return;
        }

        if(totalNumArray[arrayPos.y, arrayPos.x] != 0) // 사용자 위치에 숫자가 떠야 하는 경우
        {
            grid.ShowOverlayNum(playerPosition,true,true,totalNumArray[arrayPos.y, arrayPos.x]);

        }
        

    }

    private void SetFocus()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = TileGrid.CheckCellPosition(worldPos);

        if(cellPos == currentFocusPosition) return; // 만약 포커스가 아직 바뀌지 않았다면 요청 무시
        if(grid.boundTilemap.HasTile(cellPos))  return; // 해당 위치가 필드 바깥이면 무시
        if(isNowInputtingItem) return;

        if(CheckHasObstacle(cellPos))  // 해당 위치에 타일이 있는지 확인
        { // 만약 타일이 있다면 상호작용이 가능한 놈만 포커스를 줘야 한다. 
            // 그니까, 해당 타일의 상하좌우 4공간 상에 비어있는 곳이 하나라도 있다면 가능, 아니면 불가능
            if( (grid.boundTilemap.HasTile(cellPos +  Vector3Int.up) || CheckHasObstacle(cellPos +  Vector3Int.up)) &&
                (grid.boundTilemap.HasTile(cellPos +  Vector3Int.down) || CheckHasObstacle(cellPos +  Vector3Int.down)) &&
                (grid.boundTilemap.HasTile(cellPos +  Vector3Int.right) || CheckHasObstacle(cellPos +  Vector3Int.right)) &&
                (grid.boundTilemap.HasTile(cellPos +  Vector3Int.left) || CheckHasObstacle(cellPos +  Vector3Int.left)) 
            ) return;
            
        }
         
        grid.SetFocus(currentFocusPosition, cellPos);
        currentFocusPosition = cellPos;
    }

    private void RemoveObstacle(Vector3Int cellPos, bool special = false) // Special은 보물을 찾거나 지뢰를 없애서 갱신되고 처음 도는 재귀를 의미. 
                                                                            //이 경우에는 타일이 이미 지워져 있어도 다시 돌아야 한다
    {
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        if (special || CheckHasObstacle(cellPos))  // 해당 위치에 타일이 있는지 확인
        {
            SetFlag(cellPos, true);
            SetTreasureSearch(cellPos, true);

            RemoveObstacleTile(cellPos); 

            if(mineTreasureArray[arrayPos.y, arrayPos.x] == -1) // 지뢰
            {
                EventManager.instance.InvokeEvent(EventType.MineAppear, mineCount);
                HeartChange(-StageInformationManager.DefaultTrapDamage[(int)StageInformationManager.difficulty]);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.GetDamage);
                return;
            }else{ // 지뢰가 아닌 타일 
                
                if(mineTreasureArray[arrayPos.y, arrayPos.x] == -2) //보물인 경우에는 추가 작업 해줘야 함
                {
                    mineTreasureArray[arrayPos.y, arrayPos.x] = 0; // 배열에서 보물을 지운다
                    treasureCount--;
                    UpdateArrayNum(Total_Mine_Treasure.Total); // 갱신
                    UpdateArrayNum(Total_Mine_Treasure.Treasure); // 갱신
                    UpdateArrayNum(Total_Mine_Treasure.Mine); // 갱신
                    GetItem(true);
                    EventManager.instance.InvokeEvent(EventType.TreasureAppear, cellPos);
                    EventManager.instance.InvokeEvent(EventType.TreasureAppear, treasureCount);
                    grid.ShowTotalNum(totalNumArray, totalNumMask);
                    SetPlayer_Overlay(true);
                    grid.ShowSeperateNum(mineNumArray, treasureNumArray, totalNumMask);

                    for(int aroundI =0; aroundI < aroundY.Length; aroundI++)
                        {
                            for(int aroundJ =0; aroundJ < aroundX.Length; aroundJ++)
                            {
                                if(aroundX[aroundJ] == 0 && aroundY[aroundI] == 0) continue;

                                int x = arrayPos.x + aroundX[aroundJ];
                                int y = arrayPos.y + aroundY[aroundI];

                                if(x > -1 && x < width 
                                && y > -1 && y < height
                                && (totalNumArray[y,x] == 0)
                                && (mineTreasureArray[y,x] >= 0)
                                ) 
                                {
                                    BombObstacle(new Vector3Int(x - startX, startY - y), true);
                                }
                            }
                        }
                }
                
                if(totalNumArray[arrayPos.y, arrayPos.x] == 0){ // 완전 빈 공간인 경우 사방 8개를 자동으로 다 연다
                    for(int aroundI =0; aroundI < aroundY.Length; aroundI++)
                        {
                            for(int aroundJ =0; aroundJ < aroundX.Length; aroundJ++)
                            {
                                if(aroundX[aroundJ] == 0 && aroundY[aroundI] == 0) continue;

                                int x = arrayPos.x + aroundX[aroundJ];
                                int y = arrayPos.y + aroundY[aroundI];

                                if(x > -1 && x < width 
                                && y > -1 && y < height) 
                                {
                                    RemoveObstacle(new Vector3Int(x - startX, startY - y));
                                }
                            }
                        }
                }
            }
            
        }
    }

    private void BombObstacle(Vector3Int cellPos, bool special = false) // Special은 보물을 찾거나 지뢰를 없애서 갱신되고 처음 도는 재귀를 의미. 
                                                                        //이 경우에는 타일이 이미 지워져 있어도 다시 돌아야 한다
    {
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        if (special || CheckHasObstacle(cellPos))  // 해당 위치에 타일이 있는지 확인
        {
            SetFlag(cellPos, true);
            SetTreasureSearch(cellPos, true);

            RemoveObstacleTile(cellPos, true);

            if(mineTreasureArray[arrayPos.y, arrayPos.x] == -2) // 보물
            {
                EventManager.instance.InvokeEvent(EventType.TreasureDisappear, treasureCount);
                EventManager.instance.InvokeEvent(EventType.Game_Over, GameOver_Reason.TreasureCrash);
                return;
            }else{ // 보물이 아님
                
                if(mineTreasureArray[arrayPos.y, arrayPos.x] == -1) // 지뢰
                {
                    mineTreasureArray[arrayPos.y, arrayPos.x] = 0; // 배열에서 지뢰를 지운다
                    mineCount--;
                    UpdateArrayNum(Total_Mine_Treasure.Total); // 갱신
                    UpdateArrayNum(Total_Mine_Treasure.Mine); // 갱신
                    UpdateArrayNum(Total_Mine_Treasure.Treasure); // 갱신
                    EventManager.instance.InvokeEvent(EventType.MineDisappear, cellPos);
                    EventManager.instance.InvokeEvent(EventType.MineDisappear, mineCount);
                    grid.ShowTotalNum(totalNumArray, totalNumMask);
                    SetPlayer_Overlay(true);
                    grid.ShowSeperateNum(mineNumArray, treasureNumArray, totalNumMask);

                    // 새로 갱신 후에는 , 갱신으로 인해 자기 주변에서 새로 0이 된 것이 없나 따로 확인 절차가 필요하다
                    for(int aroundI =0; aroundI < aroundY.Length; aroundI++)
                        {
                            for(int aroundJ =0; aroundJ < aroundX.Length; aroundJ++)
                            {
                                if(aroundX[aroundJ] == 0 && aroundY[aroundI] == 0) continue;

                                int x = arrayPos.x + aroundX[aroundJ];
                                int y = arrayPos.y + aroundY[aroundI];

                                if(x > -1 && x < width 
                                && y > -1 && y < height
                                && (totalNumArray[y,x] == 0)
                                && (mineTreasureArray[y,x] >= 0)
                                ) 
                                {
                                    BombObstacle(new Vector3Int(x - startX, startY - y), true);
                                }
                            }
                        }

                }

                if(totalNumArray[arrayPos.y, arrayPos.x] == 0){ // 완전 빈 공간인 경우 사방 8개를 자동으로 다 연다
                    for(int aroundI =0; aroundI < aroundY.Length; aroundI++)
                        {
                            for(int aroundJ =0; aroundJ < aroundX.Length; aroundJ++)
                            {
                                if(aroundX[aroundJ] == 0 && aroundY[aroundI] == 0) continue;

                                int x = arrayPos.x + aroundX[aroundJ];
                                int y = arrayPos.y + aroundY[aroundI];

                                if(x > -1 && x < width 
                                && y > -1 && y < height) 
                                {
                                    BombObstacle(new Vector3Int(x - startX, startY - y));
                                }
                            }
                        }
                }
            }
            
        }
    }
    private Vector3Int ChangeCellPosToArrayPos(Vector3Int cellPos)
    {   
        return new Vector3Int(cellPos.x + startX , startY - cellPos.y, cellPos.z);
    }
    private bool CheckHasObstacle(Vector3Int cellPos)
    {
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        if(arrayPos.x <0 || arrayPos.y < 0 || arrayPos.x >= width || arrayPos.y >= height) return false;

        return !isObstacleRemoved[arrayPos.y, arrayPos.x];
    }

    private void RemoveObstacleTile(Vector3Int cellPos, bool isBomb = false)
    {
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        isObstacleRemoved[arrayPos.y, arrayPos.x] = true;
        grid.RemoveObstacleTile(cellPos, isBomb);
    }

    private void ChangeTotalToSeperate(Vector3Int cellPos)
    {
        if(!isDungeon) return;
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        if(CheckHasObstacle(cellPos)) return; // 해당 위치에 장애물 타일이 있으면 그 자리에서 반환
        if(totalNumArray[arrayPos.y, arrayPos.x] == 0) return; // 만약 해당 위치가 0이어도 반환 (써도 의미가 없음)
        if(totalNumMask[arrayPos.y, arrayPos.x]) return;

        magGlassCount--;
        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Use, Item.Mag_Glass, magGlassCount);

        totalNumMask[arrayPos.y, arrayPos.x] = true;
        SetPlayer_Overlay(true);
        grid.UpdateSeperateNum(mineNumArray, treasureNumArray, cellPos);

    }

    public void SetFlag()
    {
        SetFlag(currentFocusPosition);

    }

    private void SetFlag(Vector3Int cellPos, bool forceful = false)
    {
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        if(!(CheckHasObstacle(cellPos))) return; // 해당 위치에 장애물 타일이 없으면 무시

            if(isTutorial && tutorialStage == 1 && isDungeon && TutorialGuide.tutorialTextindex == 2)
            {
                EventManager.instance.Invoke_TutorialTextTriggerEvent();
            }

        if(forceful)
        {
            flagArray[arrayPos.y, arrayPos.x] = 0;
            grid.SetFlag(cellPos, Flag.None);
        }else
        {
            GameAudioManager.instance.PlaySFXMusic(SFXAudioType.flag);
            Flag[] flagEnumArray = (Flag[]) Enum.GetValues(typeof(Flag));
            flagArray[arrayPos.y, arrayPos.x] = (flagArray[arrayPos.y, arrayPos.x] + 1) % (flagEnumArray.Length - 1);
            grid.SetFlag(cellPos, flagEnumArray[flagArray[arrayPos.y, arrayPos.x]]);
        }
    }

    private void SetTreasureSearch(Vector3Int cellPos, bool forceful = false)
    {
        Vector3Int arrayPos = ChangeCellPosToArrayPos(cellPos);
        if(!(CheckHasObstacle(cellPos))) return; // 해당 위치에 장애물 타일이 없으면 무시
        if(treasureSearchMask[arrayPos.y, arrayPos.x] && !forceful) return;

        if(forceful)
        {
            grid.SetTreasureSearch(cellPos, TreasureSearch.None);
        }else
        {
            holyWaterCount--;
            EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Use, Item.Holy_Water, holyWaterCount);

            if(mineTreasureArray[arrayPos.y, arrayPos.x] == -2) // 보물
            {
                // 보물이 맞다고 해당 장애물 위에 띄움
                grid.SetTreasureSearch(cellPos, TreasureSearch.Yes);
            }else
            {
                // 보물이 아니라고 해당 장애물 위에 띄움
                grid.SetTreasureSearch(cellPos, TreasureSearch.No);
            }

            treasureSearchMask[arrayPos.y, arrayPos.x] = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    private void GetItem(bool isUsable)
    {
        GameAudioManager.instance.PlaySFXMusic(SFXAudioType.GetItem);
        // 만약 스테이지 모드라면 얻는 아이템은 반드시 돋보기
        if(StageInformationManager.getGameMode() == GameModeType.stage)
        {
            magGlassCount += 1;
            EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Mag_Glass, magGlassCount,1);
            return;
        }


        if(isUsable)
        {
            // 돋보기가 가장 확률이 높고, 다음이 성수, 그 다음이 포션으로 하자
            int randNum = UnityEngine.Random.Range(1, 11);
            Item randUsableItem;
            if(randNum <= 5) // 1~5 : 돋보기
            {
                randUsableItem = Item.Mag_Glass;
            }else if(randNum > 5 && randNum <= 8)  // 6~8 : 성수
            {
                randUsableItem = Item.Holy_Water;
            }else // 9~10 : 포션
            {
                randUsableItem = Item.Potion;
            }

            if(UnityEngine.Random.value < StageInformationManager.noItemRatio[(int)StageInformationManager.difficulty])
            {
                randUsableItem = Item.None;
            }   

            int obtainCount = 1;
            obtainCount += EquippedItem.canObtainPlusItem(Item.ALL_PercentageUP);
            obtainCount += EquippedItem.canObtainPlusItem(randUsableItem);



            switch(randUsableItem)
            {
                case Item.Potion :
                    potionCount += obtainCount;
                    EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, randUsableItem, potionCount,obtainCount);
                    break;
                case Item.Mag_Glass :
                    magGlassCount += obtainCount;
                    EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, randUsableItem, magGlassCount,obtainCount);
                    break;
                case Item.Holy_Water :
                    holyWaterCount += obtainCount;
                    EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, randUsableItem, holyWaterCount,obtainCount);
                    break;
                case Item.None :
                    EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, randUsableItem, holyWaterCount,0);
                    break;
            }

        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void RestPlaceInitialize(Vector3Int treasurePosition, int maxHeart = 9,  int currentHeart = 1, int potionCount = 5, int magGlassCount = 20, int holyWaterCount = 5, int totalTime = 300)
    {
        if(StageInformationManager.currentStageIndex < 5 && !(SceneManager.GetActiveScene().name == "Tutorial Last"))
        {
            GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.RestRoom);
        }else
        {
            GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.LastRoom);
        }

        EventManager.instance.UpdateLeftPanel_Invoke_Event();

        EventManager.instance.InvokeEvent(EventType.None, 0);

        isDungeon = StageInformationManager.isnextStageDungeon;
        BigTreasurePosition = treasurePosition;

        isNowInitializing = true;

        this.maxHeart = maxHeart;
        this.currentHeart = currentHeart;
        EventManager.instance.Reduce_HeartInvokeEvent(currentHeart, maxHeart);

        this.potionCount = potionCount;
        this.magGlassCount = magGlassCount;
        this.holyWaterCount = holyWaterCount;
        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Potion, potionCount);
        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Mag_Glass, magGlassCount);
        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Holy_Water, holyWaterCount);

        this.totalTime = totalTime;
        EventManager.instance.TimerInvokeEvent(0, totalTime);

        isNowInitializing = false;
        PlayerManager.instance.SetPlayerPositionStart();    

        EventManager.instance.StairOpen_Invoke_Event(); 
    }


    [Button]
    public void DungeonInitialize(int parawidth = DefaultX ,  int paraheight = DefaultY, Difficulty difficulty = Difficulty.Hard, int maxHeart = 3,  int currentHeart = 2, int potionCount = 0, int magGlassCount = 0, int holyWaterCount = 0, int totalTime = 300)
    {
        isDungeon = StageInformationManager.isnextStageDungeon;
        if(isTutorial)
        {
            GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.Tutorial);
        }else
        {
            switch(StageInformationManager.currentStagetype)
            {
                case 0 :
                    GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.Cave);
                    break;
                case 1 :
                    GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.Crypt);
                    break;
                case 2 :
                    GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.Ruin);
                    break;
            }
            
        }
        
        
        EventManager.instance.UpdateLeftPanel_Invoke_Event();

        isNowInitializing = true;

        totalNumArray = null;
        totalNumMask = null;
        treasureSearchMask = null;

        mineNumArray = null;
        treasureNumArray = null;

        this.maxHeart = maxHeart;
        this.currentHeart = currentHeart;

        startX = -1;
        startY = -1;
        if(isTutorial)
        {
            width = StageInformationManager.tutorialWidth[StageInformationManager.currentStageIndex];
            height = StageInformationManager.tutorialHeight[StageInformationManager.currentStageIndex];
        }else{
            width = parawidth;
            height = paraheight;
        }

        Debug.Log("width : " + width + " height : " + height);

        flagArray = new int[height, width];
        isObstacleRemoved = new bool[height, width];
        
        int difficultytemp = (int)StageInformationManager.difficulty;

        if(StageInformationManager.getGameMode() == GameModeType.stage)
        {
            this.potionCount = potionCount ;
            this.magGlassCount = magGlassCount ;
            this.holyWaterCount = holyWaterCount;
        }else
        {
            this.potionCount = potionCount + EquippedItem.Heart_StageBonus + StageInformationManager.plusPotion_Default_perStage[difficultytemp];
            this.magGlassCount = magGlassCount + EquippedItem.Glass_StageBonus + StageInformationManager.plusMag_Default_perStage[difficultytemp];
            this.holyWaterCount = holyWaterCount+ EquippedItem.Holy_StageBonus + StageInformationManager.plusHoly_Default_perStage[difficultytemp];
        }

        EventManager.instance.Reduce_HeartInvokeEvent(currentHeart, maxHeart);

        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Potion, this.potionCount);
        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Mag_Glass, this.magGlassCount);
        EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Obtain, Item.Holy_Water, this.holyWaterCount);
        
        EventManager.instance.UpdateLeftPanel_Invoke_Event();
        
        MakeMineTreasureArray(width, height, difficulty);

        UpdateArrayNum(Total_Mine_Treasure.Total);
        UpdateArrayNum(Total_Mine_Treasure.Mine);
        UpdateArrayNum(Total_Mine_Treasure.Treasure);

        grid.ShowEnvironment(width, height);
        grid.ShowTotalNum(totalNumArray, totalNumMask);
        grid.ShowMineTreasure(mineTreasureArray);


        RemoveObstacle(new Vector3Int(0,0,0));

        CameraSize_Change.ChangeCameraSizeFit();

        timerCoroutine = StartCoroutine(StartTimer(totalTime + EquippedItem.Time_StageBonus + StageInformationManager.DefaultTimeperStage[(int)StageInformationManager.difficulty])); 

        PlayerManager.instance.SetPlayerPositionStart();

        isNowInitializing = false;
    }

    IEnumerator StartTimer(int totalTime)
    {
        
        if(isTutorial && tutorialStage < 4)
        {
            this.totalTime = 0;
            timeElapsed = 0;
            EventManager.instance.TimerInvokeEvent(timeElapsed, timeLeft);
            yield break;
        }else
        {
            this.totalTime = totalTime;
            timeElapsed = 0;
            EventManager.instance.TimerInvokeEvent(timeElapsed, timeLeft);
        }

        while(timeLeft > 0)
        {
            yield return new WaitForSeconds(1);

            if(!isStageInputBlocked)
            {
                timeElapsed++;
                EventManager.instance.TimerInvokeEvent(timeElapsed, timeLeft);
            }
            
        }

        EventManager.instance.InvokeEvent(EventType.Game_Over, GameOver_Reason.TimeOver);
    }

    [Button]
    void UpdateArrayNum(Total_Mine_Treasure numMode)
    {
        int height = mineTreasureArray.GetLength(0);
        int width = mineTreasureArray.GetLength(1);

        int[,] targetNumArray = null;
        switch(numMode)
        {
            case Total_Mine_Treasure.Total :
                targetNumArray = totalNumArray;
                break;
            case Total_Mine_Treasure.Mine :
                targetNumArray = mineNumArray;
                break;
            case Total_Mine_Treasure.Treasure :
                targetNumArray = treasureNumArray;
                break;
        }

        if(targetNumArray == null)
        {
            switch(numMode)
            {
            case Total_Mine_Treasure.Total :
                totalNumArray = new int[height, width];
                totalNumMask = new bool[height, width];
                treasureSearchMask = new bool[height, width];
                targetNumArray = totalNumArray;
                break;
            case Total_Mine_Treasure.Mine :
                mineNumArray = new int[height, width];
                targetNumArray = mineNumArray;
                break;
            case Total_Mine_Treasure.Treasure :
                treasureNumArray = new int[height, width];
                targetNumArray = treasureNumArray;
                break;
            }

        }else
        {
            if(height != targetNumArray.GetLength(0) || 
                width != targetNumArray.GetLength(1))
            {
                Debug.LogError(" mineTreasureArray size and targetNumArray size dont match! \n height : " + height + " width : " + width + " \n targetNumArray.GetLength(0) : " + targetNumArray.GetLength(0) + " targetNumArray.GetLength(1) :" + targetNumArray.GetLength(1));
            }

            for(int i=0; i<height; i++)
            {
                for(int j=0; j<width; j++)
                {
                    targetNumArray[i,j] = 0;
                }
            }
        }

        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if(NumModeConditions[(int)numMode](mineTreasureArray[i,j])) // 모드에 따라 어떻게 판단 해야 할지 다르다
                {
                    for(int aroundI =0; aroundI < aroundY.Length; aroundI++)
                    {
                        for(int aroundJ =0; aroundJ < aroundX.Length; aroundJ++)
                        {
                            if(aroundX[aroundJ] == 0 && aroundY[aroundI] == 0) continue;

                            int x = j+ aroundX[aroundJ];
                            int y = i+ aroundY[aroundI];

                            if(x > -1 && x < width 
                            && y > -1 && y < height
                            && mineTreasureArray[y,x] != -1) // 이거 지뢰인 경우를 제외하고는 다 계산을 해줘야 한다
                            {
                                targetNumArray[y,x]++;
                            }
                        }
                    }
                }
            }
        }

    }


    [Button]
    public void MakeMineTreasureArray(int width = 10, int height = 10, Difficulty difficulty = Difficulty.Easy)
    {
        CalcStartArea(width, height, out startX, out startY);

        if(isTutorial && tutorialStage < 4)
        {
            switch (tutorialStage)
            {
                case 1 : 
                    mineTreasureArray = (int[,])StageInformationManager.tutorial1Stageinform.Clone();
                    mineCount = StageInformationManager.tutorialmineCount[tutorialStage-1];
                    treasureCount = StageInformationManager.tutorialtreasureCount[tutorialStage-1];
                    break;
                case 2 :
                    mineTreasureArray = (int[,])StageInformationManager.tutorial2Stageinform.Clone();
                    mineCount = StageInformationManager.tutorialmineCount[tutorialStage-1];
                    treasureCount = StageInformationManager.tutorialtreasureCount[tutorialStage-1];
                    break;
                case 3 :
                    mineTreasureArray = (int[,])StageInformationManager.tutorial3Stageinform.Clone();
                    mineCount = StageInformationManager.tutorialmineCount[tutorialStage-1];
                    treasureCount = StageInformationManager.tutorialtreasureCount[tutorialStage-1];
                    break;
            }

            EventManager.instance.InvokeEvent(EventType.MineAppear, mineCount);
            EventManager.instance.InvokeEvent(EventType.TreasureAppear, treasureCount);

            return;
        }


        mineTreasureArray = new int[height, width];
        int totalBockNum = height * width;

        int stageType = StageInformationManager.currentStagetype;
        
        float mineRatio = 0;
        switch(difficulty)
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

        int totalCount = (int)(totalBockNum * mineRatio);
        if(StageInformationManager.getGameMode() == GameModeType.stage)
        {
            mineCount = (int)(totalCount * (1 - StageInformationManager.StageModemineToTreasureRatio[stageType]));
        }else
        {
            mineCount = (int)(totalCount * (1 - StageInformationManager.mineToTreasureRatio));
        }
        
        treasureCount = totalCount - mineCount;

        EventManager.instance.InvokeEvent(EventType.MineAppear, mineCount);
        EventManager.instance.InvokeEvent(EventType.TreasureAppear, treasureCount);

        // 처음 시작하는 곳 0,0 근처 8칸은 폭탄이 없음을 보장한다
        mineTreasureArray[startY-1, startX-1] = 1;
        mineTreasureArray[startY-1, startX] = 1;
        mineTreasureArray[startY-1, startX+1] = 1;
        mineTreasureArray[startY, startX -1] = 1;
        mineTreasureArray[startY, startX] = 1;
        mineTreasureArray[startY, startX +1] = 1;
        mineTreasureArray[startY+1, startX-1] = 1;
        mineTreasureArray[startY+1, startX] = 1;
        mineTreasureArray[startY+1, startX+1] = 1;
        // 처음 시작하는 곳 0,0 근처 8칸은 폭탄이 없음을 보장한다

        System.Random rng = new System.Random();

        List<int> randomNumbers = Enumerable.Range(0, totalBockNum-1)
                                     .OrderBy(_ => rng.Next())
                                     .ToList();
                                     
        List<int> selectedRandomNumbers = new List<int>();

        for(int i=0; selectedRandomNumbers.Count < totalCount && i< randomNumbers.Count; i++)
        {
            int num = randomNumbers[i];

            int row = num / width;
            int column = num % width;

            if(mineTreasureArray[row, column] > 0) // 만약 지뢰 안전 구역이라면 패스
            {
                continue;
            }

            selectedRandomNumbers.Add(num);
        }

        if(selectedRandomNumbers.Count != totalCount) Debug.LogError("sleectedRandomNumbers.Count != mineCount");

        int treasureTemp = treasureCount;
        foreach(int num in selectedRandomNumbers)
        {
            int row = num / width;
            int column = num % width;

            if(treasureTemp < 1)
            {
                mineTreasureArray[row, column] = -1; // 함정
            }else
            {
                mineTreasureArray[row, column] = -2; // 보물
                treasureTemp--;
            }

        }
    }

    private void GameOver(bool isGameOver, GameOver_Reason reason)
    {
        if(isGameOver)
        {
            EquippedItem.ClearEquippedItem();

            if(isNowInputtingItem)
            {
                EventManager.instance.ItemPanelShow_Invoke_Event(Vector3Int.zero, false);
                isNowInputtingItem = false;
            }
            
            stageInputBlock++;
            if(timerCoroutine != null) StopCoroutine(timerCoroutine);
            reStartEnable = true;
            timerCoroutine = null;
        }else
        {
            StageInformationManager.currentStageIndex = 0;
            StageInformationManager.NextWidth = StageInformationManager.stageWidthMin[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
            StageInformationManager.NextHeight= StageInformationManager.stageHeightMin[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
                
            StageInformationManager.setHearts(); 
            StageInformationManager.setUsableItems(); 
            StageInformationManager.NexttotalTime = StageInformationManager.DefaultTimeforEntireGame;
            EventManager.instance.UpdateRightPanel_Invoke_Event();

            int[] usableItems = StageInformationManager.getUsableItems();
            int[] hearts = StageInformationManager.getHearts();
            
            DungeonInitialize(StageInformationManager.NextWidth, StageInformationManager.NextHeight ,StageInformationManager.difficulty ,hearts[0], hearts[1], usableItems[0], usableItems[1], usableItems[2], StageInformationManager. NexttotalTime);
            stageInputBlock =0;
        }
        
    }

    public bool hasTrapInPosition(Vector3Int position){

        if(!isDungeon) return false;
        
        Vector3Int arrayPos = ChangeCellPosToArrayPos(position);
        if(mineTreasureArray[arrayPos.y, arrayPos.x] == -1){
            return true;
        }else
        {
            return false;
        }
    }

    bool reStartEnable = false;
    public void RestartGame()
    {
        if(!reStartEnable) return;
        reStartEnable = false;
        EventManager.instance.InvokeEvent(EventType.Game_Restart);
    }

    private void MaxHeartUP()
    {
        if(maxHeart == 9) return;

        maxHeart += 3;
        currentHeart += 3;
        EventManager.instance.Heal_HeartInvokeEvent( currentHeart, maxHeart, true);

    }
    private void HeartChange(int changeValue)
    {
        currentHeart += changeValue;
        if(currentHeart < 0) currentHeart = 0;
        if(currentHeart > maxHeart) currentHeart = maxHeart;

        if(changeValue <0)
        {
            EventManager.instance.Reduce_HeartInvokeEvent( currentHeart, maxHeart);
        }else if(changeValue > 0)
        {
            EventManager.instance.Heal_HeartInvokeEvent( currentHeart, maxHeart);
        }
        

        if(currentHeart == 0)
        {
            EventManager.instance.InvokeEvent(EventType.Game_Over, GameOver_Reason.Heart0);
        }
    }

    void CalcStartArea(int width, int height, out int groundstartX,out int groundendY)
    {
        groundstartX = (width/2);

        if(height % 2 == 0)
        {
            groundendY = (height/2 -1);
        }else
        {
            groundendY = (height/2);
        }
    }
}
