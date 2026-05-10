using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load by scene name
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Load by build index
    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    // Reload current scene
    public void ReloadScene()
    {
        var current = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(current.buildIndex);
    }

    // Quit game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
