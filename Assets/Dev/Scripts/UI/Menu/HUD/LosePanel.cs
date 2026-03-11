using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LosePanel : Menu
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _rebornButton;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _continueButton.onClick.AddListener(() =>
            gameManager.GetComponent<GameState>().SetState(GameStates.ReadyState));
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    protected override void Initialized()
    {
        base.Initialized();
        CloseMenu();
    }
}
