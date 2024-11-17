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
    public static StageManager instance;
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
                EventManager.instance.Invoke_StageClearEvent();   
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

    public bool isPotionEnable()
    {
        return (potionCount > 0) && !isStageInputBlocked;
    }

    public bool isMagGlassEnable()
    {
        return (magGlassCount > 0) && !isFoucusOnObstacle &&  !isStageInputBlocked;
    }

    public bool isHolyWaterEnable()
    {
        return (holyWaterCount > 0) && !isStageInputBlocked;
    }

    public bool isFlagEnable()
    {
        return isFoucusOnObstacle && !isStageInputBlocked;
    }

    public bool isShovelEnable()
    {
        return !isStageInputBlocked;
    }

    public bool isCrashEnable()
    {
        return isFoucusOnObstacle && !isStageInputBlocked;
    }


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
            return holyWaterCount > 0 && isFoucusOnObstacle;
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
    }

    private void Start() {
        MakeScreenBlack.Clear();

        int difficulty = (int)StageInformationManager.difficulty;        
        int stageType = StageInformationManager.currentStagetype;

        StageInformationManager.setHearts(3,3); 
        StageInformationManager.setUsableItems(0,StageInformationManager.StageMagItemAmount[stageType,difficulty] , 0);
        StageInformationManager.NexttotalTime = StageInformationManager.StageModeTime[stageType,difficulty];

        int[] usableItems = StageInformationManager.getUsableItems();
        int[] hearts = StageInformationManager.getHearts();

        DungeonInitialize(StageInformationManager.StageModestageWidth[stageType], StageInformationManager.StageModestageHeight[stageType] ,
        StageInformationManager.difficulty ,hearts[0], hearts[1], 
        usableItems[0], usableItems[1], usableItems[2], 
        StageInformationManager. NexttotalTime);

        
    }

    public void ItemPanelShow(bool flag)
    {
        if(flag)
        {
            if(!isNowInputtingItem)
            {
                if(!interactOkflag) return;

                isNowInputtingItem = true;
                EventManager.instance.ItemPanelShow_Invoke_Event(currentFocusPosition, true, holyWaterEnable, isFoucusOnObstacle, magGlassEnable , potionEnable);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.itemMenuShow);  

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
            if(isNowInputtingItem) return;
            if(shovelLock) return;
            
            
            
            if(!isNearFlag)
            {
                EventManager.instance.AfterMoveCallBackEvent += RemoveObstacle;
                MoveToCurrentFocusPosition();
            }else
            {
                RemoveObstacle();
            }
            
        }else
        {
            MoveToCurrentFocusPosition();
        }
    }

    public void MoveToCurrentFocusPosition()
    {
        if(isNowInputtingItem) return;
        
        Vector3Int warpTarget = currentFocusPosition;

        Vector3Int warpTarget_down = new Vector3Int(currentFocusPosition.x, currentFocusPosition.y-1, currentFocusPosition.z);
        Vector3Int warpTarget_left = new Vector3Int(currentFocusPosition.x -1, currentFocusPosition.y, currentFocusPosition.z);
        Vector3Int warpTarget_up = new Vector3Int(currentFocusPosition.x, currentFocusPosition.y+1, currentFocusPosition.z);
        Vector3Int warpTarget_right = new Vector3Int(currentFocusPosition.x+1, currentFocusPosition.y, currentFocusPosition.z);

        if(!grid.boundTilemap.HasTile(warpTarget) && !TileGrid.CheckObstaclePosition(warpTarget) && !hasTrapInPosition(warpTarget))
        {
            warpTarget = currentFocusPosition;
        }else if(!grid.boundTilemap.HasTile(warpTarget_down) && !TileGrid.CheckObstaclePosition(warpTarget_down)&& !hasTrapInPosition(warpTarget_down))
        {
            warpTarget = warpTarget_down;
        }else if(!grid.boundTilemap.HasTile(warpTarget_left) && !TileGrid.CheckObstaclePosition(warpTarget_left) && !hasTrapInPosition(warpTarget_left))
        {
            warpTarget = warpTarget_left;

        }else if(!grid.boundTilemap.HasTile(warpTarget_up) && !TileGrid.CheckObstaclePosition(warpTarget_up) && !hasTrapInPosition(warpTarget_up))
        {
            warpTarget = warpTarget_up;
        }else if(!grid.boundTilemap.HasTile(warpTarget_right) && !TileGrid.CheckObstaclePosition(warpTarget_right) && !hasTrapInPosition(warpTarget_right))
        {
            warpTarget = warpTarget_right;
        }else{
            return;
        }

        if(hasTrapInPosition(warpTarget)) return;
        InputManager.InputEvent.Invoke_Move(warpTarget);

    }

    private void Update() {

        if(EventSystem.current.IsPointerOverGameObject()) return;
        if(isNowInitializing) return;

        SetPlayer_Overlay();
        if(grid)
        {
            grid.SetInteract_OkAuto();
        }
    }

    private void OnEnable() {
        EventManager.instance.Game_Over_Event += GameOver;
        EventManager.instance.ItemUseEvent += ItemUse;
        EventManager.instance.SetFocusEvent += SetFocus;

        EventManager.instance.UpdateRightPanel_Invoke_Event();
    }

    private void OnDisable() {
        EventManager.instance.Game_Over_Event -= GameOver;
        EventManager.instance.ItemUseEvent -= ItemUse;
        EventManager.instance.SetFocusEvent -= SetFocus;
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
                if(holyWaterCount <= 0) return;
                if(!CheckHasObstacle(currentFocusPosition)) return;

                if(!isNearFlag)
                {
                    EventManager.instance.AfterMoveCallBackEvent += Use_Holy_Water;
                    MoveToCurrentFocusPosition();
                }else{
                    Use_Holy_Water();
                }

                
            break;
            case ItemUseType.Crash :
                if(!CheckHasObstacle(currentFocusPosition)) return;

                if(!isNearFlag)
                {
                    EventManager.instance.AfterMoveCallBackEvent += Use_Pickaxe;
                    MoveToCurrentFocusPosition();
                }else{
                    Use_Pickaxe();
                }

            break;
            case ItemUseType.Mag_Glass :
                if(magGlassCount <= 0) return;
                if(CheckHasObstacle(currentFocusPosition)) return; // 해당 위치에 장애물 타일이 있으면 그 자리에서 반환
                Vector3Int arrayPos = ChangeCellPosToArrayPos(currentFocusPosition);
                if(totalNumArray[arrayPos.y, arrayPos.x] == 0) return; // 만약 해당 위치가 0이어도 반환 (써도 의미가 없음)
                if(totalNumMask[arrayPos.y, arrayPos.x]) return;

                if(gapBetweenPlayerFocus.magnitude != 0)
                {
                    EventManager.instance.AfterMoveCallBackEvent += Use_MagGlass;
                    MoveToCurrentFocusPosition();

                }else
                {
                    Use_MagGlass();
                }
                
            break;
            case ItemUseType.Potion :
                if(potionCount <= 0) return;
                potionCount--;
                EventManager.instance.Item_Count_Change_Invoke_Event(EventType.Item_Use, Item.Potion, potionCount);
                HeartChange(1);
                GameAudioManager.instance.PlaySFXMusic(SFXAudioType.potion);
            break;
            case ItemUseType.Shovel :
                MoveOrShovelOrInteract();
            break;
        }
    }

    private void Use_Holy_Water()
    {
        SetTreasureSearch(currentFocusPosition);
        GameAudioManager.instance.PlaySFXMusic(SFXAudioType.HolyWater);

        EventManager.instance.AfterMoveCallBackEvent -= Use_Holy_Water;
    }

    private void Use_Pickaxe()
    {
        BombObstacle(currentFocusPosition);
        GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Bomb);

        EventManager.instance.AfterMoveCallBackEvent -= Use_Pickaxe;
    }

    private void Use_MagGlass()
    {
        ChangeTotalToSeperate(currentFocusPosition);
        GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Mag_Glass);

        EventManager.instance.AfterMoveCallBackEvent -= Use_MagGlass;
    }
    

    private void RemoveObstacle()
    {
        if(!isNearFlag) return;
        GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Shovel);
        RemoveObstacle(currentFocusPosition);     

        EventManager.instance.AfterMoveCallBackEvent -= RemoveObstacle;
    }
    
    private void SetInteract_Ok()
    {

        //여기에 그냥 전체 블럭을 뒤져보고, 근처에 빈 공간이 하나라도 있다면 자기를 빛나게 하자
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

    private void SetFocus(Vector3 mousePos)
    {
        float screenWidth = Screen.width;
        float ignoreMargin = 180f;

        if (mousePos.x < ignoreMargin || mousePos.x > screenWidth - ignoreMargin)
        {
            Debug.Log("Mouse position is in the ignored margin area.");
            return; 
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
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

        if(TutorialGuide.bNowTutorial && TutorialGuide.tutorialIndex == 1)
        {
            EventManager.instance.TutorialShowEvent.Invoke();
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

        if(isUsable)
        {
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

    [Button]
    public void DungeonInitialize(int parawidth = DefaultX ,  int paraheight = DefaultY, Difficulty difficulty = Difficulty.Hard, int maxHeart = 3,  int currentHeart = 2, int potionCount = 0, int magGlassCount = 0, int holyWaterCount = 0, int totalTime = 300)
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

        width = TutorialGuide.bNowTutorial ? 7 : parawidth;
        height = TutorialGuide.bNowTutorial ? 7 : paraheight;

        Debug.Log("width : " + width + " height : " + height);

        flagArray = new int[height, width];
        isObstacleRemoved = new bool[height, width];
        
        int difficultytemp = (int)StageInformationManager.difficulty;

        this.potionCount = TutorialGuide.bNowTutorial ? 1 : potionCount;
        this.magGlassCount = magGlassCount ;
        this.holyWaterCount = TutorialGuide.bNowTutorial ? 1 : holyWaterCount;


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

        timerCoroutine = StartCoroutine(StartTimer(totalTime)); 

        PlayerManager.instance.SetPlayerPositionStart();

        isNowInitializing = false;
    }

    IEnumerator StartTimer(int totalTime)
    {
        this.totalTime = totalTime;
        timeElapsed = 0;
        EventManager.instance.TimerInvokeEvent(timeElapsed, timeLeft);

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
        mineCount = (int)(totalCount * (1 - StageInformationManager.StageModemineToTreasureRatio[stageType]));

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
            if(isNowInputtingItem)
            {
                EventManager.instance.ItemPanelShow_Invoke_Event(Vector3Int.zero, false);
                isNowInputtingItem = false;
            }
            
            stageInputBlock++;
            if(timerCoroutine != null) StopCoroutine(timerCoroutine);
            reStartEnable = true;
            timerCoroutine = null;
        }
        
    }

    public bool hasTrapInPosition(Vector3Int position){        
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
