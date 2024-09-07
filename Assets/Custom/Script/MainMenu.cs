using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameModeType
{
    adventure = 0,
    stage = 1,
    None = 2,
    tutorial = 3,
    
}

public class MainMenu : MonoBehaviour , AlertCallBack
{
    public class RestartManageClass
    {
        public static GameModeType restartGameModeType = GameModeType.None;
    }

    public Image showImage;
    public Sprite[] showImages;
    public Image PanelColor;
    public Color[] colors;

    string[] loadAdventureSceneName = {"Cave Dungeon", "Crypt Dungeon", "Ruin Dungeon" };
    Animator animator;

    public delegate void AlertCallBackDelgate();
    private AlertCallBackDelgate callbackFunction;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        int mode = (int)StageInformationManager.getGameMode();
        ChangeSceneNum(StageInformationManager.currentStagetype);
        StageInformationManager.SetDataInitialState();
    }


    private void Start() {
        if(RestartManageClass.restartGameModeType != GameModeType.None)
        {
            switch(RestartManageClass.restartGameModeType)
            {
                case GameModeType.stage :
                    StartStage();
                    break;
            }
            RestartManageClass.restartGameModeType = GameModeType.None;
        }else
        {
            MakeScreenBlack.Clear();
            GameAudioManager.instance.PlayBackGroundMusic(BackGroundAudioType.MainMenu);
            StageInformationManager.currentStageIndex = 0;
        }
        
    }

    public void SetAnimationPlayingFlag(int flag)
    {
        if(flag == 0)
        {
            EventManager.isAnimationPlaying = false;
        }else
        {
            EventManager.isAnimationPlaying = true;
        }
        
    }

    public void ChangeSceneNum(int num)
    {
        StageInformationManager.currentStagetype = num;
        showImage.sprite = showImages[StageInformationManager.currentStagetype];
        PanelColor.color = new Color(colors[StageInformationManager.currentStagetype].r, colors[StageInformationManager.currentStagetype].g, colors[StageInformationManager.currentStagetype].b);
    }

    [SerializeField] Image difficultyPanel;
    [SerializeField] Color[] difficultyPanelColors;


    int currentShowFlag = 0; // 0 : none, 1 : tutorial, 2 : stage, 3 : setting 


    public void ShowAdventure()
    {
        if(EventManager.isAnimationPlaying) return;

        if(currentShowFlag != 0)
        {
            animator.SetTrigger("ClosePanel");
            
            currentShowFlag = 0;
        }else
        {
            animator.SetTrigger("StageShow");
            currentShowFlag = 2;
        }
    }

    public void StartStage()
    {
        MakeScreenBlack.Hide();
        LoadingInformation.loadingSceneName = loadAdventureSceneName[StageInformationManager.currentStagetype];
        SceneManager.LoadScene("Before Enter Dungeon");
    }

    public void ExitGame()
    {
        callbackFunction = QuitGame;
        AlertUI.instance.ShowAlert(AlertUI.AlertSituation.Exit, this);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void CallBack()
    {
        callbackFunction();
    }
}
