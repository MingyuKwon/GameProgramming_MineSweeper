using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class TileGrid : MonoBehaviour, IGridInterface
{
    #region  serializeFields
    [Header("Environment")]
    [SerializeField] private TileBase BaseTile;
    [SerializeField] private TileBase BoundTile;
    [SerializeField] private TileBase MineTile;
    [SerializeField] private TileBase MineDisappearTile;
    [SerializeField] private TileBase TreasureTile;
    [SerializeField] private TileBase ObstacleTile;
    [SerializeField] private TileBase FocusTile;
    [SerializeField] private TileBase TreasureYesTile;
    [SerializeField] private TileBase TreasureNoTile;

    [SerializeField] private TileBase CrackTile;
    [SerializeField] private TileBase BombTile;

    [Space]
    [Space]
    [SerializeField] private TileBase interactOkTile;

    [Header("Flag")]
    [SerializeField] private TileBase TrapFlag;
    [SerializeField] private TileBase TreasureFlag;
    [SerializeField] private TileBase QuestionFlag;
    

    [Space]
    [Header("Num")]
    [SerializeField] private TileBase[] totalNum;
    [SerializeField] private TileBase[] mineNum;
    [SerializeField] private TileBase[] treasureNum;
    #endregion

    [SerializeField] private Tilemap[] tilemaps; 
    //0 : Base , 1: Bound , 2 : Total Num, 3 : Bomb Num, 4 : Treasure Num,5 : Mine and Treasure ,
    //6 : Obstacle ,7 : treasure search ,8 : Flag ,9 : Interact Ok, 10 : Crack, 11 : Focus
    //12 : Overlay_total ,13 : Overlay_mine ,14 : Overlay_treasure

    
 
    static Tilemap staticObstacleTileMap;
    public static Vector3Int CheckCellPosition(Vector3 worldPos)
    {
        return staticObstacleTileMap.WorldToCell(worldPos);;
    }

    public static bool CheckObstaclePosition(Vector3Int worldPos)
    {
        return staticObstacleTileMap.HasTile(worldPos);
    }
    
    public static Vector3 CheckWorldPosition(Vector3Int worldPos)
    {
        return staticObstacleTileMap.CellToWorld(worldPos) + staticObstacleTileMap.cellSize * 0.5f;
    }

    public Tilemap obstacleTilemap{
        get{
            return tilemaps[6];
        }
    }

    public Tilemap boundTilemap{
        get{
            return tilemaps[1];
        }
    }

    private void OnEnable() {
        EventManager.instance.SetAnimationTileEvent += ReserveAnimation;
        staticObstacleTileMap = tilemaps[6];
    }

    private void OnDisable() {
        EventManager.instance.SetAnimationTileEvent -= ReserveAnimation;
        staticObstacleTileMap = null;
    }

    [Button]
    public void ShowEnvironment(int width = 10, int height = 10)
    {
        foreach(Tilemap a in tilemaps)
        {
            a.ClearAllTiles();
        }

        int groundstartX;
        int groundendX;
        int groundstartY;
        int groundendY;

        CalcBoxStart(width, height, out groundstartX, out groundendX, out groundstartY, out groundendY);

        BoxFillCustom(tilemaps[0], BaseTile, groundstartX-1, groundstartY-1, groundendX+1, groundendY+1);

        int padding = 20;

        int borderstartX = groundstartX - padding;
        int borderendX = groundendX + padding;
        int borderstartY = groundstartY - padding;
        int borderendY = groundendY + padding;

        BoxFillCustom(tilemaps[1], BoundTile, borderstartX, borderstartY, groundstartX-1, borderendY);
        BoxFillCustom(tilemaps[1], BoundTile, groundendX + 1, borderstartY, borderendX, borderendY);
        BoxFillCustom(tilemaps[1], BoundTile, borderstartX, borderstartY, borderendX, groundstartY - 1);
        BoxFillCustom(tilemaps[1], BoundTile, borderstartX, groundendY + 1, borderendX, borderendY);

        BoxFillCustom(tilemaps[6], ObstacleTile, groundstartX, groundstartY, groundendX, groundendY);
    }

    public void ShowOverlayNum(Vector3Int cellPosition, bool isSet, bool isTotal,  int firstValue = -1, int SecondValue = -1)
    {
        if(isSet)
        {
            if(isTotal)
            {
                tilemaps[12].SetTile(cellPosition, totalNum[firstValue]);
            }else
            {
                tilemaps[13].SetTile(cellPosition, mineNum[firstValue]);
                tilemaps[14].SetTile(cellPosition, treasureNum[SecondValue]);
            }
        }else // 타일 지우기
        {
            tilemaps[12].SetTile(cellPosition, null);
            tilemaps[13].SetTile(cellPosition, null);
            tilemaps[14].SetTile(cellPosition, null);
        }
    }


    [Button]
    public void ShowTotalNum(int[,] totalNumArray, bool[,] totalNumMask)
    {
        tilemaps[2].ClearAllTiles();
        tilemaps[12].ClearAllTiles();

        int height = totalNumArray.GetLength(0);
        int width = totalNumArray.GetLength(1);
        
        int groundstartX;
        int groundendX;
        int groundstartY;
        int groundendY;

        CalcBoxStart(width, height, out groundstartX, out groundendX, out groundstartY, out groundendY);


        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if(totalNumArray[i,j] > 0 && totalNumArray[i,j] < 9 && !totalNumMask[i,j])
                {   
                    tilemaps[2].SetTile(new Vector3Int(j + groundstartX,-i + groundendY,0) , totalNum[totalNumArray[i,j]]);
                }
            }
        }
    }

    [Button]
    public void ShowMineTreasure(int[,] mineTreasureArray)
    {
        tilemaps[5].ClearAllTiles();

        int height = mineTreasureArray.GetLength(0);
        int width = mineTreasureArray.GetLength(1);
        
        int groundstartX;
        int groundendX;
        int groundstartY;
        int groundendY;

        CalcBoxStart(width, height, out groundstartX, out groundendX, out groundstartY, out groundendY);


        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if(mineTreasureArray[i,j] < 0)
                {   
                    tilemaps[5].SetTile(new Vector3Int(j + groundstartX,-i + groundendY,0) , (mineTreasureArray[i,j] == -1) ? MineTile : TreasureTile);
                }
            }
        }
    }

    [Button]
    public void UpdateSeperateNum(int[,] mineNumArray, int[,] treasureNumArray, Vector3Int position)
    {
        int height = mineNumArray.GetLength(0);
        int width = mineNumArray.GetLength(1);
        
        int groundstartX;
        int groundendX;
        int groundstartY;
        int groundendY;

        CalcBoxStart(width, height, out groundstartX, out groundendX, out groundstartY, out groundendY);

        int x = position.x - groundstartX;
        int y = -position.y + groundendY;

        tilemaps[2].SetTile(new Vector3Int(position.x,position.y,0) , null);
        tilemaps[3].SetTile(new Vector3Int(position.x,position.y,0) , mineNum[mineNumArray[y,x]]);
        tilemaps[4].SetTile(new Vector3Int(position.x,position.y,0) , treasureNum[treasureNumArray[y,x]]);

        // 만약 전체 필드를 다 보여주고 싶다면
        // for(int i=0; i<height; i++)
        // {
        //     for(int j=0; j<width; j++)
        //     {
        //         if(bombNumArray[i,j] >= 0 && bombNumArray[i,j] < 9)
        //         {   
        //             tilemaps[3].SetTile(new Vector3Int(j + groundstartX,-i + groundendY,0) , mineNum[bombNumArray[i,j]]);
        //         }

        //         if(treasureNumArray[i,j] >= 0 && treasureNumArray[i,j] < 9)
        //         {   
        //             tilemaps[4].SetTile(new Vector3Int(j + groundstartX,-i + groundendY,0) , treasureNum[treasureNumArray[i,j]]);
        //         }
        //     }
        // }
    }

    public void ShowSeperateNum(int[,] mineNumArray, int[,] treasureNumArray, bool[,] totalNumMask)
    {
        int height = mineNumArray.GetLength(0);
        int width = mineNumArray.GetLength(1);
        
        int groundstartX;
        int groundendX;
        int groundstartY;
        int groundendY;

        CalcBoxStart(width, height, out groundstartX, out groundendX, out groundstartY, out groundendY);

        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if(totalNumMask[i,j]) // 여기가 true로 되어 있으면 아이템으로 토탈 지움 -> seperate 띄워저 있음
                {   
                    tilemaps[3].SetTile(new Vector3Int(j + groundstartX,-i + groundendY,0) , mineNum[mineNumArray[i,j]]);
                    tilemaps[4].SetTile(new Vector3Int(j + groundstartX,-i + groundendY,0) , treasureNum[treasureNumArray[i,j]]);
                }
            }
        }
        
    }

    public void RemoveObstacleTile(Vector3Int cellPos, bool isBomb = false)
    {
        if(obstacleTilemap.HasTile(cellPos)) {
            StageManager.stageInputBlock++;
            StartCoroutine(crackAnimation(cellPos, isBomb));
        }
    }

    IEnumerator crackAnimation(Vector3Int cellPos, bool isBomb = false)
    {
        if(isBomb)
        {
            tilemaps[10].SetTile(cellPos, BombTile);
        }else
        {
            tilemaps[10].SetTile(cellPos, CrackTile);
        }

        if(StageManager.isNowInitializing)
        {
            obstacleTilemap.SetTile(cellPos, null);
            tilemaps[10].SetTile(cellPos, null);
        }

        yield return new WaitForSeconds(0.2f);

        if(!StageManager.isNowInitializing)
        {
            obstacleTilemap.SetTile(cellPos, null);
            tilemaps[10].SetTile(cellPos, null);
        }

        StageManager.stageInputBlock--;
    }

    public void ReserveAnimation(EventType tileType, Vector3Int cellPos )
    {
        StartCoroutine(SetAnimationTile(cellPos, tileType));
    }

    IEnumerator SetAnimationTile(Vector3Int cellPos, EventType tileType)
    {
        while(StageManager.isStageInputBlocked){
            yield return new WaitForEndOfFrame();
        }
        StageManager.stageInputBlock++;

        yield return null;

        switch(tileType)
        {
            case EventType.MineDisappear :
                tilemaps[5].SetTile(cellPos , MineDisappearTile);
                break;
            case EventType.TreasureAppear :
                tilemaps[5].SetTile(cellPos , TreasureTile);
                break;
        }

        yield return new WaitForSeconds(0.5f);
        tilemaps[5].SetTile(cellPos , null);

        StageManager.stageInputBlock--;
    }

    public void SetInteract_Ok(Vector3Int[] before , Vector3Int[] after)
    {
        for(int i=0; i<5; i++)
        {
            tilemaps[9].SetTile(before[i], null);
        }

        for(int i=0; i<5; i++)
        {
            if(before[i] == Vector3Int.forward) continue;
            tilemaps[9].SetTile(after[i], interactOkTile);
        }
        
    }

    public void SetFlag(Vector3Int position , Flag flag)
    {
        switch(flag)
        {
            case Flag.None :
                tilemaps[8].SetTile(position, null);
                break;
            case Flag.Question :
                tilemaps[8].SetTile(position, QuestionFlag);
                break;
            case Flag.Mine :
                tilemaps[8].SetTile(position, TrapFlag);
                break;
            case Flag.Treasure :
                tilemaps[8].SetTile(position, TreasureFlag);
                break;
            
        }
    }

    public void SetTreasureSearch(Vector3Int position , TreasureSearch flag)
    {
        switch(flag)
        {
            case TreasureSearch.None :
                tilemaps[7].SetTile(position, null);
                break;
            case TreasureSearch.Yes :
                tilemaps[7].SetTile(position, TreasureYesTile);
                break;
            case TreasureSearch.No :
                tilemaps[7].SetTile(position, TreasureNoTile);
                break;
        }
    }

    public void SetFocus(Vector3Int previousPosition , Vector3Int newPosition)
    {
        tilemaps[11].SetTile(previousPosition, null);
        tilemaps[11].SetTile(newPosition, FocusTile);
    }

    private void BoxFillCustom(Tilemap tilemap, TileBase tile, int startX, int startY, int endX, int endY)
    {
        for(int i=startX; i <= endX; i++)
        {
            for(int j = startY; j <= endY; j++)
            {
                tilemap.SetTile(new Vector3Int(i,j,0), tile);
            }
        }
        
    }

    private void CalcBoxStart(int width, int height, out int groundstartX, out int groundendX,out int groundstartY,out int groundendY)
    {
        if(width % 2 == 0)
        {
            groundstartX = -(width/2);
            groundendX = (width/2 - 1);
        }else
        {
            groundstartX = -(width/2);
            groundendX = (width/2);
        }

        if(height % 2 == 0)
        {
            groundstartY = -(height/2);
            groundendY = (height/2 -1);
        }else
        {
            groundstartY = -(height/2);
            groundendY = (height/2);
        }
    }
}
