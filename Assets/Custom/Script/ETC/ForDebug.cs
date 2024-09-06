using UnityEngine;
using Sirenix.OdinInspector;

public class ForDebug : MonoBehaviour
{
    [Button]
    public void DeletePlayerPrefabs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
