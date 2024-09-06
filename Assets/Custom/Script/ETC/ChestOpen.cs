using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    [SerializeField] Sprite[] chestSprites;
    SpriteRenderer spriteRenderer;


    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        EventManager.instance.ObtainBigItemEvent += OpenChest;
        EventManager.instance.StageClearEvent += OpenChest;
    }

    private void OnDisable() {
        EventManager.instance.ObtainBigItemEvent -= OpenChest;
        EventManager.instance.StageClearEvent -= OpenChest;
    }


    [Button]
    private void OpenChest()
    {
        StartCoroutine(openChestAnimation());
    }

    IEnumerator openChestAnimation()
    {
        spriteRenderer.sprite = chestSprites[0];
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = chestSprites[1];
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = chestSprites[2];
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = chestSprites[3];
    }
}
