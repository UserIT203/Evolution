using UnityEngine;
using Zenject;

public class GameWinState : FSMState
{
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;

    private CanvasGroup _winPanel;

    public GameWinState(FSM fsm, CanvasGroup winPanel) : base(fsm)
    {
        _winPanel = winPanel;
    }


    public override void Enter()
    {
        base.Enter();

        _winPanel.alpha = 1f;
        _winPanel.interactable = true;
        _winPanel.blocksRaycasts = true;

        _waveManager.RestartWaves();
    }

    public override void Exit()
    {
        _winPanel.alpha = 0f;
        _winPanel.interactable = false;
        _winPanel.blocksRaycasts = false;
    }
}
