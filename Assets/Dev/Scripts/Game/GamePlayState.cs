using UnityEngine;
using Zenject;

public class GamePlayState : FSMState
{
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;

    private CanvasGroup _gamePlayUI;

    public GamePlayState(FSM fsm, CanvasGroup gamePlayUI) : base(fsm)
    {
        _gamePlayUI = gamePlayUI;
    }

    public override void Enter()
    {
        Debug.Log("[SET: PLAY STATE]");

        _gamePlayUI.alpha = 1.0f;
        _gamePlayUI.interactable = true;
        _gamePlayUI.blocksRaycasts = true;

        _gameManager.Play();
        _waveManager.SetWave();
    }
}
