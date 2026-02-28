using UnityEngine;


[RequireComponent (typeof(LevelUpgrade))]
[RequireComponent(typeof(LevelView))]
public class LevelObserver : MonoBehaviour
{
    private LevelUpgrade _levelModel;
    private LevelView _levelView;

    private void OnDisable()
    {
        _levelModel.onChangeMoney -= _levelView.ChangeCointText;
    }

    private void Awake()
    {
        _levelModel = GetComponent<LevelUpgrade>();
        _levelView = GetComponent<LevelView>();

        _levelModel.onChangeMoney += _levelView.ChangeCointText;
    }
}
