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

    }

    public void SetState(GameStates state)
    {
        switch (state)
        {
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
