using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

[RequireComponent (typeof(CanvasGroup))]
public class UpgradesMenu : Menu, ILevelHandler
{
    [Inject] private DiContainer _DIContainer;

    [Header("Unit Links")]
    [SerializeField] private Transform _unitUpgradesContainer;
    [SerializeField] private UnitCard _unitUpgradesCardTemplate;

    [Header("Game Modifier")]
    [SerializeField] private TMP_Text _damageModifier;
    [SerializeField] private TMP_Text _healthModifier;
    [SerializeField] private TMP_Text _speedModifier;

    [Header("Game Upgrade UI Links")]
    [SerializeField] private TMP_Text _upgradeValue;
    [SerializeField] private Button _moneyPerSeconsUpgrade;

    private GlobalManager _globalManager;
    private LevelUpgrade _levelUpgrade;
    private GameManager _gameManager;
    private UnitCard[] _unitUpgradesCards;

    [Inject]
    public void Construct(LevelUpgrade levelUpgrade, GlobalManager globalManager, GameManager gameManager)
    {
        _levelUpgrade = levelUpgrade;
        _globalManager = globalManager;
        _gameManager = gameManager;
        _levelUpgrade.onUpgradeMoneyPerSecond += UpdateInfoInUpgradeMoneyPerSecondButton;
    }

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();

        UpdateUpgradeCardInfo();
        UpdateModifierInfo();
    }

    private void OnEnable()
    {
        _moneyPerSeconsUpgrade.onClick.AddListener(
            () => _levelUpgrade.UpgradeMoneyPerSecond());
    }

    private void OnDisable()
    {
        _moneyPerSeconsUpgrade.onClick.RemoveAllListeners();
    }

    public override void Initialized()
    {
        Debug.Log("Init Upgrade Menu");
        _unitUpgradesCards = new UnitCard[_gameManager.UnitInfo.Length];

        if(_unitUpgradesContainer.childCount > 0)
        {
            for (int i = 0; i < _unitUpgradesContainer.childCount; i++)
                Destroy(_unitUpgradesContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < _gameManager.UnitInfo.Length; i++)
        {
            UnitCard card = Instantiate(_unitUpgradesCardTemplate);
            _DIContainer.Inject(card);
            card.transform.SetParent(_unitUpgradesContainer, false);
            _unitUpgradesCards[i] = card;
        }

        UpdateUpgradeCardInfo();

        _levelUpgrade.onUpgradeMoneyPerSecond += UpdateInfoInUpgradeMoneyPerSecondButton;

        _moneyPerSeconsUpgrade.GetComponentInChildren<TMP_Text>().text
            = _levelUpgrade.CurrentGameModifier.Cost.ToString();

        _upgradeValue.text = _levelUpgrade.CurrentGameModifier.Modifier.ModifierValue.ToString();
    }

    private void UpdateUpgradeCardInfo()
    {
        for (int i = 0; i < _unitUpgradesCards.Length; i++)
        {
            _unitUpgradesCards[i].UpdateInfo(_gameManager.UnitInfo[i]);
        }
    }

    private void UpdateInfoInUpgradeMoneyPerSecondButton(GameModifier modifier)
    {
        _moneyPerSeconsUpgrade.GetComponentInChildren<TMP_Text>().text =
            modifier.Cost.ToString();
        _upgradeValue.text =
            string.Format("{0:f}", _levelUpgrade.CurrentGameModifier.Modifier.ModifierValue);
    }

    private void UpdateModifierInfo()
    {
        _damageModifier.text = $"x{_globalManager.DamageMultiplier.GetValue()}";
        _healthModifier.text = $"x{_globalManager.HealthMultiplier.GetValue()}";
        _speedModifier.text = $"x{_globalManager.SpeedMultiplier.GetValue()}";
    }

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        
    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        Initialized();
    }
}
