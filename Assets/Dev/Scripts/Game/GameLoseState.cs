using UnityEngine;
using Zenject;

public class GameLoseState: FSMState
{
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;

    private CanvasGroup _losePanel;

    public GameLoseState(FSM fsm, CanvasGroup losePanel) : base(fsm)
    {
        _losePanel = losePanel;
    }

    public override void Enter()
    {
        base.Enter();

        _losePanel.alpha = 1f;
        _losePanel.interactable = true;
        _losePanel.blocksRaycasts = true;

        _waveManager.RestartWaves();
    }

    public override void Exit()
    {
        _losePanel.alpha = 0f;
        _losePanel.interactable = false;
        _losePanel.blocksRaycasts = false;
    }
}
