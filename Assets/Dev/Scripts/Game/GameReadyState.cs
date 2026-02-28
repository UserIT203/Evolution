using UnityEngine;

public class GameReadyState : FSMState
{
    private CanvasGroup _readyCanvas;

    public GameReadyState(FSM fsm, CanvasGroup readyCanvas) : base(fsm)
    {
        _readyCanvas = readyCanvas;
    }

    public override void Enter()
    {
        _readyCanvas.alpha = 1;
        _readyCanvas.interactable = true;
        _readyCanvas.blocksRaycasts = true;
    }

    public override void Exit()
    {
        _readyCanvas.alpha = 0;
        _readyCanvas.interactable = false;
        _readyCanvas.blocksRaycasts = false;
    }
}
