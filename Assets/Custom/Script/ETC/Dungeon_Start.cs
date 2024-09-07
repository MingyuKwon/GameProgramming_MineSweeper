using UnityEngine.SceneManagement;
using UnityEngine;

public class Dungeon_Start : MonoBehaviour
{
    public string moveAutoSceneName = "null";
    void Start()
    {
        if(moveAutoSceneName == "null")
        {
            StageInformationManager.isnextStageDungeon = true;
        }else
        {
            LoadingInformation.loadingSceneName = moveAutoSceneName;
        }
        

        SceneManager.LoadScene("Loading");
    }

}
