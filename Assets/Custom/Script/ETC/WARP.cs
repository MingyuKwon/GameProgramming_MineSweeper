using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WARP : MonoBehaviour
{
    public string loadSceneName;
    public string LastStageSceneName;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;

    public Sprite[] openStairSprite;

    private void Awake() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        EventManager.instance.StairOpenEvent += warpEnable;
    }

    private void OnDisable() {
        EventManager.instance.StairOpenEvent -= warpEnable;
    }

    private void warpEnable()
    {
        if(circleCollider2D.enabled) return;

        StartCoroutine(openningStair());
    }

    IEnumerator openningStair()
    {
        EventManager.isAnimationPlaying = true;

        yield return new WaitForSeconds(0.3f);
        MainCameraScript.CameraForcePosition = transform.position;

        spriteRenderer.sprite = openStairSprite[0];
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.sprite = openStairSprite[1];
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.sprite = openStairSprite[2];
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.sprite = openStairSprite[3];
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.sprite = openStairSprite[4];
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.sprite = openStairSprite[5];

        circleCollider2D.enabled = true;
        MainCameraScript.CameraForcePosition = Vector3.forward;
        
        EventManager.isAnimationPlaying = false;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        SceneManager.LoadScene("Loading");
        
        if(StageInformationManager.currentStageIndex < 5)
        {
            LoadingInformation.loadingSceneName = loadSceneName;
        }else
        {
            LoadingInformation.loadingSceneName = LastStageSceneName;
        }
        
        
    }
}
