using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _play;
    [SerializeField] private Button _build;
    [SerializeField] private Button _quit;
    [SerializeField] private string _spaceSceneName;
    [SerializeField] private string _buildSceneName;



    void Start()
    {
        _play.onClick.AddListener(LoadSpaceScene);
        _build.onClick.AddListener(LoadBuildScene);
        _quit.onClick.AddListener(QuitGame);
    }



    private void LoadSpaceScene()
    {
        SceneManager.LoadScene(_spaceSceneName);
    }
    private void LoadBuildScene()
    {
        SceneManager.LoadScene(_buildSceneName);
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}
