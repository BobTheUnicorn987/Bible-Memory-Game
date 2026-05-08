using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel;
    private SceneManager _sceneManager;
    private bool _showMenu;

    private void Start()
    {
        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        _showMenu = false;
        menuPanel.SetActive(false);
    }

    public void ToggleMenu()
    {
        _showMenu = !_showMenu;
        Time.timeScale = _showMenu ? 0 : 1;
        menuPanel.SetActive(_showMenu);
    }

    public void ToMainMenu()
    {
        _sceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        // TODO: Settings
    }

    public void Quit()
    {
        _sceneManager.QuitGame();
    }
}
