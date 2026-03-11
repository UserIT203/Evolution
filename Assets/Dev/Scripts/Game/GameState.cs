using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameState : MonoBehaviour
{
    [Inject] private DiContainer _diContainer;

    [Header("Links For Game Play State")]
    [SerializeField] private CanvasGroup _gamePlayUI;

    private FSM _fsm;

    private MenuManager _menuManager;

    [Inject]
    public void Construct(MenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    private void Awake()
    {
        InitializedFSM();
    }

    private void Update()
    {
        _fsm.Update();
    }

    private void InitializedFSM()
    {
        _fsm = new FSM();

        _fsm.AddFsm(new GameReadyState(_fsm, _menuManager));

        GamePlayState gamePlayState = new GamePlayState(_fsm, _gamePlayUI);
        _diContainer.Inject(gamePlayState);
        _fsm.AddFsm(gamePlayState);

        GameWinState winState = new GameWinState(_fsm, _menuManager);
        _diContainer.Inject(winState);
        _fsm.AddFsm(winState);

        GameLoseState loseState = new GameLoseState(_fsm, _menuManager);
        _diContainer.Inject(loseState);
        _fsm.AddFsm(loseState);

        _fsm.SetState<GameReadyState>();
    }

    public void SetState(GameStates state)
    {
        switch (state)
        {
            case GameStates.ReadyState:
                _fsm.SetState<GameReadyState>();
                break;
            case GameStates.GameState:
                _fsm.SetState<GamePlayState>();
                break;
            case GameStates.WinState:
                _fsm.SetState<GameWinState>();
                break;
            case GameStates.LoseState:
                _fsm.SetState<GameLoseState>();
                break;
        }
    }
}

public enum GameStates
{
    ReadyState,
    GameState,
    PauseState,
    WinState,
    LoseState
}
