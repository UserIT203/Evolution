using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayMenu : Menu
{
    [SerializeField] private Button _playButton;

    [Inject]
    public void Constract(GameManager gameManager)
    {
        GameState gameState = gameManager.GetComponent<GameState>();
        _playButton.onClick.AddListener(() => gameState.SetState(GameStates.GameState));
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

}
