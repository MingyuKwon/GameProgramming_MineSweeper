using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private void OnEnable() {
        InputManager.InputEvent.MovePressEvent += MovePlayer;
    }

    private void OnDisable() {
        InputManager.InputEvent.MovePressEvent -= MovePlayer;
    }

    private void MovePlayer(Vector3Int cellPosition)
    {
        if(TileGrid.CheckObstaclePosition(cellPosition)) return;
        Vector3Int gap = PlayerManager.instance.checkPlayerNearFourDirection(cellPosition);

        if(gap == Vector3Int.zero) return;
        
        if(gap != Vector3Int.forward)
        {
            GameAudioManager.instance.PlaySFXMusic(SFXAudioType.Move);
            if(gap.magnitude == 1) 
            {
                StartCoroutine(MoveOneDirectly(gap));
            }else
            {
                StartCoroutine(MoveTwoDirectly(gap));
            }

            
        }else
        {
            GameAudioManager.instance.PlaySFXMusic(SFXAudioType.MoveTeleport);
            StartCoroutine(MoveTeleport(cellPosition));
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

    private bool isPlayerCanTransparent(Vector3Int position)
    {
        return !TileGrid.CheckObstaclePosition(position) && !StageManager.instance.hasTrapInPosition(position);
    }   

    IEnumerator MoveTwoDirectly(Vector3Int moveVector)
    {
        StageManager.stageInputBlock++;

        Vector3Int playerTargetPosition = PlayerManager.instance.PlayerCellPosition + moveVector;

        if(moveVector == twoTileVectorPositons[0])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.right))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.right));
                yield return null;
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.right));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
            
        }else if(moveVector == twoTileVectorPositons[1])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.up))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.up));
                yield return null;
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.up));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }
        else if(moveVector == twoTileVectorPositons[2])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.down))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.down));
                yield return null;
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.down));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }
        else if(moveVector == twoTileVectorPositons[3])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.left))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.left));
                yield return null;
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.left));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }
        else if(moveVector == twoTileVectorPositons[4])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.right))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.right));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.up));
            }else if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.up))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.up));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.right));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }
        else if(moveVector == twoTileVectorPositons[5])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.right))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.right));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.down));
            }else if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.down))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.down));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.right));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }
        else if(moveVector == twoTileVectorPositons[6])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.left))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.left));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.up));
            }else if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.up))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.up));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.left));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }
        else if(moveVector == twoTileVectorPositons[7])
        {
            if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.left))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.left));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.down));
            }else if(isPlayerCanTransparent(PlayerManager.instance.PlayerCellPosition + Vector3Int.down))
            {
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.down));
                yield return StartCoroutine(MoveOneDirectly(Vector3Int.left));
            }else
            {
                yield return StartCoroutine(MoveTeleport(playerTargetPosition));
            }
        }

        StageManager.stageInputBlock--;
    }

    IEnumerator MoveOneDirectly(Vector3Int moveVector)
    {
        StageManager.stageInputBlock++;

        PlayerManager.instance.playerAnimation.MoveAnimation(moveVector);

        float elapsedTime = 0;
        float time = 0.2f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + moveVector;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;  // 마지막 위치 보정

    
        StageManager.stageInputBlock--;

        EventManager.instance.AfterMoveCallBackEvent?.Invoke();
    }

    IEnumerator MoveTeleport(Vector3Int movePosition)
    {
        PlayerManager.instance.playerAnimation.MoveAnimation(Vector3Int.forward);

        yield return new WaitForSeconds(0.2f);
        transform.position =  TileGrid.CheckWorldPosition(movePosition);
        yield return new WaitForSeconds(0.2f);
    
        StageManager.stageInputBlock--;

        EventManager.instance.AfterMoveCallBackEvent?.Invoke();
    }
}
