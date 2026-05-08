using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject settingsPanel;
    private SceneManager _sceneManager;
    private bool _showSettings;

    private void Start()
    {
        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        _showSettings = false;
        settingsPanel.SetActive(false);
    }

    public void ToggleSettings()
    {
        _showSettings = !_showSettings;
        settingsPanel.SetActive(_showSettings);
    }
}
