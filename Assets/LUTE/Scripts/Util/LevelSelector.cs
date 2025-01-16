using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    /// the exact name of the target level
    [Tooltip("the exact name of the target level")]
    public string levelName;

    public virtual void GoToLevel()
    {
        if (!string.IsNullOrEmpty(levelName))
            LoadScene(levelName);
    }
    public static void LoadScene(string newSceneName)
    {
        SceneManager.LoadScene(newSceneName);
    }

    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}