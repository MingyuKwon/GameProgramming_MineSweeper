using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraSize_Change : MonoBehaviour
{
    private static Grid Stage;
    private static Tilemap tilemap;

    private static Camera camera;

    private void Awake() {
        Stage = FindObjectOfType<Grid>();
        tilemap = Stage.transform.GetChild(0).GetComponent<Tilemap>();
        camera = GetComponent<Camera>();
    }

    public static void ChangeCameraSizeFit()
    {   
        camera.orthographicSize = tilemap.cellBounds.size.y /2 + 0.2f;
    }

}
