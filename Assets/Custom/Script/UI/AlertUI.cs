using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface AlertCallBack{
    public void CallBack();
}

public class AlertUI : MonoBehaviour
{
    public enum AlertSituation{
        Exit = 0,
        Restart = 1,
        GoBackMainMenu = 2,
        StartNewGame = 3,
        
    }

    string[] alertKorean = {
        "게임을 종료 하시겠습니까?",
        "전체 스테이지를 다시 시작하겠습니까?",
        "메인 메뉴로 돌아가시겠습니까?",
        "새로운 게임을 시작하겠습니까? \n마지막으로 저장된 데이터는 사라지게 됩니다",
        };
    string[] alertEnglish = {
    "Do you want to exit the game?",
    "Do you want to restart all stages?",
    "Do you want to return to the main menu?",
    "Do you want to start a new game? \nYour last saved data will be lost.",
    };
    

    public TextMeshProUGUI MainText;
    private AlertCallBack alertCallBack = null;

    public static AlertUI instance = null;
    // Start is called before the first frame update
    private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }

        gameObject.SetActive(false);
    }

    public void YES()
    {
        alertCallBack.CallBack();
        CloseAlert();
    }

    public void NO()
    {   
        CloseAlert();
    }

    public void ShowAlert(AlertSituation situation, AlertCallBack callBackInstance)
    {
        if(LanguageManager.currentLanguage == "English")
        {
            MainText.text = alertEnglish[(int)situation];
        }else
        {
            MainText.text = alertKorean[(int)situation];
        }
        
        alertCallBack = callBackInstance;
        gameObject.SetActive(true);
    }

    public void CloseAlert()
    {   
        gameObject.SetActive(false);
        MainText.text = "";
        alertCallBack = null;
    }
}
