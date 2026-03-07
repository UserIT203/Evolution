using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class PlayMenu : Menu
{
    [Header("Text Links")]
    [SerializeField] private TMP_Text _eraText;
    [SerializeField] private TMP_Text _levelText;

    [Header("Modificators Links")]
    [SerializeField] private TMP_Text _damageModifierText;
    [SerializeField] private TMP_Text _healthModifierText;
    [SerializeField] private TMP_Text _speedModifierText;

    [SerializeField] private Image _levelIcon;
    [SerializeField] private Button _playButton;

    private GlobalManager _globalManager;
    private EraManager _eraManager;
    private LevelManager _levelManager;

    [Inject]
    public void Constract(
        GameManager gameManager, GlobalManager globalManager, 
        EraManager eraManager, LevelManager levelManager)
    {
        GameState gameState = gameManager.GetComponent<GameState>();
        _playButton.onClick.AddListener(() => gameState.SetState(GameStates.GameState));

        _globalManager = globalManager;
        _eraManager = eraManager;
        _levelManager = levelManager;
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        FillUI();
    }

    private void FillUI()
    {
        _eraText.text = $"{_eraText.text} {_eraManager.CurrentEra +1}";
        _levelText.text = $"{_levelManager.CurrentOpenLevels}/{_levelManager.MaxLevel}";

        _damageModifierText.text = "x" + _globalManager.DamageMultiplier.GetValue().ToString();
        _healthModifierText.text = "x" + _globalManager.HealthMultiplier.GetValue().ToString();
        _speedModifierText.text = "x" + _globalManager.SpeedMultiplier.GetValue().ToString();

        _levelIcon.sprite = _levelManager.CurentLevelIcon;
    }
}
