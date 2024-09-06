using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakeScreenBlack : MonoBehaviour
{
    public static MakeScreenBlack instance;
    public TextMeshProUGUI loadingText;
    static Transform image;

    public static void Hide()
    {
        image.gameObject.SetActive(true);
    }

    public static void Clear()
    {
        image.gameObject.SetActive(false);
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

        image = transform.GetChild(0);

        StartCoroutine(LoadingText());
    }

    IEnumerator LoadingText()
    {
        while(true)
        {
            loadingText.text = "Loading";
            yield return new WaitForSecondsRealtime(0.1f);

            loadingText.text = "Loading .";
            yield return new WaitForSecondsRealtime(0.1f);

            loadingText.text = "Loading . .";
            yield return new WaitForSecondsRealtime(0.1f);

            loadingText.text = "Loading . . .";
            yield return new WaitForSecondsRealtime(0.1f);

            loadingText.text = "Loading . .";
            yield return new WaitForSecondsRealtime(0.1f);

            loadingText.text = "Loading .";
            yield return new WaitForSecondsRealtime(0.1f);
        }
        
    }

}
