using UnityEngine;
using Zenject;

public class GameLoseState: FSMState
{
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;

    private MenuManager _menuManager;

    public GameLoseState(FSM fsm, MenuManager menuManager) : base(fsm)
    {
        _menuManager = menuManager;
    }

    public override void Enter()
    {
        base.Enter();

        _menuManager.GetHUDPanel<LosePanel>().OpenMenu();

        _waveManager.RestartWaves();
    }

    public override void Exit()
    {
        _menuManager.GetHUDPanel<LosePanel>().CloseMenu();
    }
}
