using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    static int[,] resolution = {
        {1024,576},
        {1152,648},
        {1280,720 },
        {1366,768},
        {1600,900},
        {1920,1080},
        {2560,1440},
    };

    public static int windowedWidth;
    public static int windowedHeight;

    public static bool isFullScreen{
        get{
            return PlayerPrefs.GetString("isFullScreen", "No") == "Yes";
        }
        set{
            PlayerPrefs.SetString("isFullScreen", value ? "Yes" : "No");
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        // 이전에 저장된 창 모드의 해상도를 불러옵니다.
        if(PlayerPrefs.HasKey("windowedWidth") && PlayerPrefs.HasKey("windowedHeight"))
        {
            windowedWidth = PlayerPrefs.GetInt("windowedWidth"); // 기본값 예시
            windowedHeight = PlayerPrefs.GetInt("windowedHeight"); // 기본값 예시
        }else
        {
            PlayerPrefs.SetInt("windowedWidth", 1280);
            PlayerPrefs.SetInt("windowedHeight", 720);
            PlayerPrefs.Save();

            windowedWidth = 1280;
            windowedHeight = 720;
        }

        Screen.SetResolution(windowedWidth, windowedHeight,isFullScreen);
    }

    public static void SetFullScreen(bool isFullScreen)
    {
        if (isFullScreen)
        {
            // 전체 화면일 경우 현재 해상도를 저장하고 전체 화면으로 전환합니다.
            PlayerPrefs.SetInt("windowedWidth", Screen.width);
            PlayerPrefs.SetInt("windowedHeight", Screen.height);
            PlayerPrefs.Save();
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            // 창 모드로 전환할 때 이전에 저장된 해상도를 불러옵니다.
            Screen.SetResolution(windowedWidth, windowedHeight, false);
        }
    }

    public static void SetResolution(int index)
    {
        windowedWidth = resolution[index,0];
        windowedHeight = resolution[index,1];
        PlayerPrefs.SetInt("windowedWidth", windowedWidth);
        PlayerPrefs.SetInt("windowedHeight", windowedHeight);
        PlayerPrefs.Save();

        if(!isFullScreen)
        {
            Screen.SetResolution(windowedWidth, windowedHeight, false);
        }
        
    }
}
