using UnityEngine;

public class GameReadyState : FSMState
{
    private CanvasGroup _readyCanvas;
    private MenuManager _menuManager;

    public GameReadyState(FSM fsm, MenuManager menuManager) : base(fsm)
    {
        _readyCanvas = menuManager.GetComponent<CanvasGroup>();
        _menuManager = menuManager;
    }

    public override void Enter()
    {
        _readyCanvas.alpha = 1;
        _readyCanvas.interactable = true;
        _readyCanvas.blocksRaycasts = true;

        _menuManager.OpenMenu(0);
    }

    public override void Exit()
    {
        _readyCanvas.alpha = 0;
        _readyCanvas.interactable = false;
        _readyCanvas.blocksRaycasts = false;
    }
}
