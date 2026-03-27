using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayMenu : Menu, ILevelHandler
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
    private GameManager _gameManager;

    [Inject]
    public void Constract(
        GameManager gameManager, GlobalManager globalManager, 
        EraManager eraManager, LevelManager levelManager)
    {        
        _gameManager = gameManager;
        _globalManager = globalManager;
        _eraManager = eraManager;
        _levelManager = levelManager;

        _levelManager.RegisterToChange(this);
    }

    private void OnEnable()
    {
        _playButton.onClick.AddListener(() => _gameManager.Play());
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveAllListeners();
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
        _damageModifierText.text = "x" + _globalManager.DamageMultiplier.GetValue().ToString();
        _healthModifierText.text = "x" + _globalManager.HealthMultiplier.GetValue().ToString();
        _speedModifierText.text = "x" + _globalManager.SpeedMultiplier.GetValue().ToString();
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        _levelText.text = $"{_levelManager.CurrentOpenLevels}/{_levelManager.MaxLevel}";
        _levelIcon.sprite = _levelManager.CurentLevelIcon;
    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        _eraText.text = string.Format(_eraText.text, _eraManager.CurrentEra + 1);
    }
}
