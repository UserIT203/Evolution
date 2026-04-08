using Unity.VisualScripting;
using UnityEngine;
using Zenject;

[RequireComponent (typeof(GameManager))]
[RequireComponent (typeof(GameView))]
public class GameObserver : MonoBehaviour
{
    [Inject] private AbilityManager _abilityManager;

    private GameManager _gameManager;
    private GameView _gameView;

    private void OnEnable()
    {
        _gameManager = GetComponent<GameManager>();
        _gameView = GetComponent<GameView>();

        _abilityManager.onChangeAbility += _gameView.UpdateAbilityButton;
        _abilityManager.onAbilityTimer += _gameView.ChangeAbilityFill;
        
        _gameView.InitializedAbilityButton(_abilityManager.UseAbility);

        _gameManager.onChangeMoneyCount += _gameView.SetCurrentMoneyText;
        _gameManager.onChangeTime += _gameView.ChangeImageFill;
        _gameManager.onInitializedUnit += _gameView.CreateUnitCard;
        _gameManager.onEnd += _gameView.RestartUI;
    }

    private void OnDisable()
    {
        _abilityManager.onChangeAbility -= _gameView.UpdateAbilityButton;
        _abilityManager.onAbilityTimer -= _gameView.ChangeAbilityFill;

        _gameManager.onChangeMoneyCount -= _gameView.SetCurrentMoneyText;
        _gameManager.onChangeTime -= _gameView.ChangeImageFill;
        _gameManager.onInitializedUnit -= _gameView.CreateUnitCard;
        _gameManager.onEnd -= _gameView.RestartUI;
    }
}
