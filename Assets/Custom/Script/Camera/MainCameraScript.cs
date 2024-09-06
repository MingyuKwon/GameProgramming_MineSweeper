using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    Transform thisTransform;
    Transform player;

    public static Vector3 CameraForcePosition = Vector3.forward;
    private void Awake() {
        thisTransform = GetComponent<Transform>();
    }

    private void Start() {
        player = PlayerManager.instance.playerTransform;
    }
    void Update()
    {
        if(CameraForcePosition == Vector3.forward)
        {
            thisTransform.position = new Vector3(player.position.x, player.position.y,thisTransform.position.z); 
        }else
        {
            thisTransform.position = new Vector3(CameraForcePosition.x, CameraForcePosition.y,thisTransform.position.z); 
        }
    }
}
