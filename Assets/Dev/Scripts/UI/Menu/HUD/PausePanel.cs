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
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        Time.timeScale = 1f;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        Time.timeScale = 0f;
    }

    protected override void Initialized()
    {
        base.Initialized();

        _continueButton.onClick.AddListener(CloseMenu);
        _mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void GoToMainMenu()
    {
        _gameManager.EndGame();
        MenuManager.OpenUIMenu();
    }
}
