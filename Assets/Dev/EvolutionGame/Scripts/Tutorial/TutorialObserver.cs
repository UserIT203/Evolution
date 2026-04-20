using UnityEngine;

[RequireComponent (typeof(TutorialManager))]
[RequireComponent (typeof(TutorialView))]
public class TutorialObserver : MonoBehaviour
{
    private TutorialManager _manager;
    private TutorialView _view;

    private void Awake()
    {
        _manager = GetComponent<TutorialManager>();
        _view = GetComponent<TutorialView>();
    }

    private void OnEnable()
    {
        _manager.onSetTitle += _view.SetTutorialTitle;
        _manager.onSetInfo += _view.SetTutorialInfo;
        _manager.onEndStage += _view.HideTutorialCanvas;
    }

    private void OnDisable()
    {
        _manager.onSetTitle -= _view.SetTutorialTitle;
        _manager.onSetInfo -= _view.SetTutorialInfo;
        _manager.onEndStage -= _view.HideTutorialCanvas;
    }
}
