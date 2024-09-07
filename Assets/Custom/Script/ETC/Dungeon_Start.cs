using UnityEngine.SceneManagement;
using UnityEngine;

public class Dungeon_Start : MonoBehaviour
{
    public string moveAutoSceneName = "null";
    void Start()
    {
        if(moveAutoSceneName != "null")
        {
            LoadingInformation.loadingSceneName = moveAutoSceneName;
        }
    
        SceneManager.LoadScene("Loading");
    }

}
