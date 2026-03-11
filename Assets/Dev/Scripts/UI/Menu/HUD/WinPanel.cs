using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WinPanel : Menu, IDisposable
{
    [SerializeField] private Button _continueButton;

    private GameState _gameState;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameState = gameManager.GetComponent<GameState>();

        _continueButton.onClick.AddListener(() => _gameState.SetState(GameStates.ReadyState));
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

    public void Dispose()
    {
        _continueButton.onClick.RemoveAllListeners();
    }

    protected override void Initialized()
    {
        base.Initialized();
        CloseMenu();
    }
}
