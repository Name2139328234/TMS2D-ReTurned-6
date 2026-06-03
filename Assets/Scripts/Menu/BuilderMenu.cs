using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class BuilderMenu : MonoBehaviour
{
    [SerializeField] private Builder _builder;
    [SerializeField] private GameObject _gameMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private string _homeSceneName = "MainMenu";
    [SerializeField] private AudioSource _buttonHoverSound;



    void Start()
    {
        _menuButton.onClick.AddListener(EnableMenu);
        _resumeButton.onClick.AddListener(DisableMenu);
        _homeButton.onClick.AddListener(SaveAndQuit);
    }



    private void EnableMenu()
    {
        _builder.SetAllowedBuilding(false);
        _gameMenu.SetActive(false);
        _pauseMenu.SetActive(true);
    }
    private void DisableMenu()
    {
        _builder.SetAllowedBuilding(true);
        _gameMenu.SetActive(true);
        _pauseMenu.SetActive(false);
    }
    private void SaveAndQuit()
    {
        Serializer.SerializeShip(_builder.Ship);
        Serializer.SerializeInventory(_builder.Inventory);

        SceneManager.LoadScene(_homeSceneName);
    }
}
