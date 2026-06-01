using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameMenu : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Serializer _serializer;
    [SerializeField] private Button _pause;
    [SerializeField] private GameObject _pauseMenu;
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
        _serializer.SerializeShip(_player.Ship);
        _serializer.SerializeInventory(_player.Inventory);
        SceneManager.LoadScene(_mainMenuName);
    }
    private void Pause()
    {
        Time.timeScale = 0f;
        _pause.gameObject.SetActive(false);
        _pauseMenu.SetActive(true);
    }
    private void Resume()
    {
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
        _pause.gameObject.SetActive(true);
    }
}
