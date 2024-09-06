using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionPanel : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI[] OptionScreens;
    public TextMeshProUGUI[] OptionAudio;
    public TextMeshProUGUI[] OptionGeneral;

    string[] screenEnglishTexts = {"Brightness", "FullScreen", "Resolution"};
    string[] screenKoreanTexts = {"밝기", "전체화면", "해상도"};

    string[] audioEnglishTexts = {"Background", "Sound Effect", "UI", "Mute"};
    string[] audioKoreanTexts = {"배경음악", "음향 효과", "UI", "음소거"};

    string[] generalEnglishTexts = {"language"};
    string[] generalKoreanTexts = {"언어"};

    [Space]
    public Dropdown resolution;
    public Dropdown language;

    [Space]
    public Scrollbar brightness;
    public Scrollbar bgm;
    public Scrollbar sfx;
    public Scrollbar ui;

    [Space]
    public Toggle mute;
    public Toggle fullscreen;

    private void OnEnable() {
        LanguageManager.languageChangeEvent += ChangeOptionPanelText;

        switch(ResolutionManager.windowedWidth)
        {
            case 1024 :
                resolution.value = 0;
                break;
            case 1152 :
                resolution.value = 1;
                break;
            case 1280 :
                resolution.value = 2;
                break;
            case 1366 :
                resolution.value = 3;
                break;
            case 1600 :
                resolution.value = 4;
                break;
            case 1920 :
                resolution.value = 5;
                break;
            case 2560 :
                resolution.value = 6;
                break;
        }

        switch(LanguageManager.currentLanguage)
        {
            case "Korean" :
                language.value = 0;
                break;
            case "English" :
                language.value = 1;
                break;
        }

        brightness.value = BrightnessManager.brightness;

        bgm.value = GameAudioManager.currentBackGroundVolume;
        sfx.value = GameAudioManager.currentSFXVolume;
        ui.value = GameAudioManager.currentUIVolume;

        mute.isOn = !(GameAudioManager.totalVolme > 0);
        fullscreen.isOn = ResolutionManager.isFullScreen;
        
    }

    private void OnDisable() {
        LanguageManager.languageChangeEvent -= ChangeOptionPanelText;
    }


    public void setFullScreen(bool isOn)
    {
        ResolutionManager.isFullScreen = isOn;
        ResolutionManager.SetFullScreen(ResolutionManager.isFullScreen);
    }

    public void ResolutionChange(Dropdown dropdown)
    {
        int value = dropdown.value;
        ResolutionManager.SetResolution(value);
    }
    public void LanguageChange(Dropdown dropdown)
    {
        string[] strings = {"Korean", "English"};
        int value = dropdown.value;
        LanguageManager.SaveLanguage(strings[value]);
        LanguageManager.Invoke_languageChangeEvent(strings[value]);
    }

    public void ChangeOptionPanelText(string s)
    {
        string[] tempScreenText = screenEnglishTexts;
        string[] tempAudioText = audioEnglishTexts;
        string[] tempGeneralText = generalEnglishTexts;

        if(LanguageManager.currentLanguage == "Korean")
        {
            tempScreenText = screenKoreanTexts;
            tempAudioText = audioKoreanTexts;
            tempGeneralText = generalKoreanTexts;
        }else if(LanguageManager.currentLanguage == "English")
        {
            tempScreenText = screenEnglishTexts;
            tempAudioText = audioEnglishTexts;
            tempGeneralText = generalEnglishTexts;
        }

        for(int i=0; i<OptionScreens.Length; i++)
        {
            OptionScreens[i].text = tempScreenText[i];
        }

        for(int i=0; i<OptionAudio.Length; i++)
        {
            OptionAudio[i].text = tempAudioText[i];
        }

        for(int i=0; i<OptionGeneral.Length; i++)
        {
            OptionGeneral[i].text = tempGeneralText[i];
        }
    }

    public void BrightnessChange(float value)
    {
        BrightnessManager.instance.setBrightNess(value);
    }

    public void BackGroundSoundChange(float value)
    {
        GameAudioManager.currentBackGroundVolume = value;
    }

    public void SFXSoundChange(float value)
    {
        GameAudioManager.currentSFXVolume = value;
    }

    public void UISoundChange(float value)
    {
        GameAudioManager.currentUIVolume = value;
    }

    public void Mute(bool isOn)
    {
        if(isOn)
        {
            GameAudioManager.totalVolme = 0;
        }else
        {
            GameAudioManager.totalVolme = 1;
        }
        
    }
}
