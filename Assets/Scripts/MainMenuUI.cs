using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;
    public Button verseListButton;
    public Button exitButton;
    
    private SceneManager _sceneManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        
        playButton.onClick.AddListener(() => _sceneManager.LoadScene("Play"));
        verseListButton.onClick.AddListener(() => _sceneManager.LoadScene("VerseList"));
        exitButton.onClick.AddListener(() => _sceneManager.QuitGame());
    }
}
