using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PausePanel : Menu
{
    [Inject] private GameManager _gameManager;

    [Header("Pause UI Links")]
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _mainMenuButton;

    public override void CloseMenu()
    {
        _canvasGroup.Hide();

        Time.timeScale = 1f;
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();

        Time.timeScale = 0f;
    }

    public override void Initialized()
    {
        base.Initialized();

        _continueButton.onClick.AddListener(CloseMenu);
        _mainMenuButton.onClick.AddListener(GoToMainMenu);
        _settingButton.onClick.AddListener(OpenSettingMenu);   
    }

    private void GoToMainMenu()
    {
        _gameManager.EndGame();
        MenuManager.OpenUIMenu();
    }

    private void OpenSettingMenu()
    {
        MenuManager.GetHUDPanel<SettingMenu>().OpenMenu();
    }
}
