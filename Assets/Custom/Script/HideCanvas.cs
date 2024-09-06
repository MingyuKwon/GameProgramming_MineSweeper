using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HideCanvas : MonoBehaviour
{
    public Text loadingText;
    public Slider loadingBar;
    private void Awake() {
        StartCoroutine(LoadingText());

        LoadScene(LoadingInformation.loadingSceneName);
        LoadingInformation.loadingSceneName = null;
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
        }
        
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private void savePlayerDataDuringLoading()
    {
        if(StageInformationManager.isnextStageDungeon && 
        StageInformationManager.getGameMode() == GameModeType.adventure&&
        LoadingInformation.loadingSceneName != "Main Menu" && 
        LoadingInformation.loadingSceneName != "Tutorial 1"&& 
        LoadingInformation.loadingSceneName != "Tutorial 2"&& 
        LoadingInformation.loadingSceneName != "Tutorial 3"&& 
        LoadingInformation.loadingSceneName != "Tutorial 4"&& 
        LoadingInformation.loadingSceneName != "Tutorial Last"
        )
        {
            //그럼 여기는 튜토리얼이 아닌, 다음이 스테이지인 경우에만 호출이 되게 된다
            if(StageInformationManager.NextWidth == -1) // 만약 아직 미리 크기 설정이 안되어 있다면
            {
                StageInformationManager.NextWidth = StageInformationManager.stageWidthMin[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
                StageInformationManager.NextHeight= StageInformationManager.stageHeightMin[(int)StageInformationManager.difficulty,StageInformationManager.currentStageIndex];
                // 먼저 초기화를 해주고 나서 저장에 들어간다
            }
            PlayerSaveManager.instance.SavePlayerStageData();
        }else
        {
            if(LoadingInformation.loadingSceneName == "Main Menu" && StageInformationManager.getGameMode() != GameModeType.stage) // 메인 메뉴으로 가기 전에 가장 마지막에 저장한 값으로 세팅
            {
                // 가장 최근에 저장되었던 값으로 다시 초기화하고 
                StageInformationManager.setPlayerData(PlayerSaveManager.instance.GetPlayerStageData());
            }
            loadingBar.gameObject.SetActive(false);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        savePlayerDataDuringLoading();

        loadingBar.value = 0;
        yield return new WaitForSeconds(0.05f);
        loadingBar.value = 0.25f;
        yield return new WaitForSeconds(0.05f);
        loadingBar.value = 0.5f;
        yield return new WaitForSeconds(0.05f);
        loadingBar.value = 0.75f;
        yield return new WaitForSeconds(0.05f);
        loadingBar.value = 1f;

        SceneManager.LoadScene(sceneName);
        
    }

}
