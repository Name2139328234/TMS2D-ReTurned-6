using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameMenu : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameMenu;
    [SerializeField] private Button _pause;
    [SerializeField] private Button _resume;
    [SerializeField] private Button _home;
    [SerializeField] private string _mainMenuName;



    void Start()
    {
        _home.onClick.AddListener(Home);
        _pause.onClick.AddListener(Pause);
        _resume.onClick.AddListener(Resume);
    }



    private void Home()
    {
        Serializer.SerializeShip(_player.Ship);
        Serializer.SerializeInventory(_player.Inventory);
        SceneManager.LoadScene(_mainMenuName);
    }
    private void Pause()
    {
        Time.timeScale = 0f;
        _gameMenu.SetActive(false);
        _pauseMenu.SetActive(true);
    }
    private void Resume()
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
        _gameMenu.SetActive(true);
    }
}
