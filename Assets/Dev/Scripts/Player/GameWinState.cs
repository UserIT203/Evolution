using UnityEngine;
using Zenject;

public class GameWinState : FSMState
{
    [Inject] private GameManager _gameManager;
    [Inject] private WaveManager _waveManager;

    private MenuManager _menuManager;

    public GameWinState(FSM fsm, MenuManager menuManager) : base(fsm)
    {
        _menuManager = menuManager;
    }

    public override void Enter()
    {
        base.Enter();

        _menuManager.GetHUDPanel<WinPanel>().OpenMenu();
        _waveManager.RestartWaves();
    }

    public override void Exit()
    {
        _menuManager.GetHUDPanel<WinPanel>().CloseMenu();
    }
}
