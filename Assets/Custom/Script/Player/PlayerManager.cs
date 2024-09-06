using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerAnimation playerAnimation;
    public PlayerMove playerMove;

    public Transform playerTransform;

    public void SetPlayerPositionStart()
    {
        playerTransform.position = new Vector3(0.5f, 0.5f, 0);
    }

    public Vector3Int PlayerCellPosition{
        get {
            return TileGrid.CheckCellPosition(transform.position);
        }
    }

    private Vector3Int[] twoTileVectorPositons = new Vector3Int[8] 
    {
         new Vector3Int(2,0,0),
         new Vector3Int(0,2,0) ,
         new Vector3Int(0,-2,0) , 
         new Vector3Int(-2,0,0) ,
         new Vector3Int(1,1,0) ,
         new Vector3Int(1,-1,0), 
         new Vector3Int(-1,1,0),
         new Vector3Int(-1,-1,0)
    };
    public Vector3Int checkPlayerNearFourDirection(Vector3Int checkPosition){
        Vector3Int gap = checkPosition - PlayerCellPosition;

        if(gap.magnitude == 1 ||
            gap == twoTileVectorPositons[0] || 
            gap == twoTileVectorPositons[1] || 
            gap == twoTileVectorPositons[2] || 
            gap == twoTileVectorPositons[3] || 
            gap == twoTileVectorPositons[4] ||  
            gap == twoTileVectorPositons[5] || 
            gap == twoTileVectorPositons[6] || 
            gap == twoTileVectorPositons[7] 
        )
        {
            return gap;
        }else if(gap.magnitude == 0)
        {
            return Vector3Int.zero;
        }

        return Vector3Int.forward;
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

        playerAnimation = GetComponent<PlayerAnimation>();
        playerMove = GetComponent<PlayerMove>();
        playerTransform = GetComponent<Transform>();

        playerTransform.position = new Vector3(0.5f, 0.5f, 0);
    }

    private void OnEnable() {
        EventManager.instance.BackToMainMenuEvent += DestroyPlayer;
    }

    private void OnDisable() {
        EventManager.instance.BackToMainMenuEvent -= DestroyPlayer;
    }

    public void DestroyPlayer()
    {
        Destroy(this.gameObject);
    }

}
