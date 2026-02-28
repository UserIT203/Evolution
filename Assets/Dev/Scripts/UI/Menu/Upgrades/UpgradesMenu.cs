using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent (typeof(CanvasGroup))]
public class UpgradesMenu : Menu, ILevelHandler
{
    [Inject] private DiContainer _DIContainer;

    [Header("Unit Links")]
    [SerializeField] private Transform _unitUpgradesContainer;
    [SerializeField] private UnitCard _unitUpgradesCardTemplate;

    [Header("Game Upgrade UI Links")]
    [SerializeField] private TMP_Text _coinCountText;
    [SerializeField] private Button _moneyPerSeconsUpgrade;

    private UnitBase[] _playerUnits;
    private LevelUpgrade _levelUpgrade;
    private UnitCard[] _unitUpgradesCards;

    [Inject]
    public void Construct(LevelUpgrade levelUpgrade)
    {
        _levelUpgrade = levelUpgrade;
        
        _levelUpgrade.onUpgradeMoneyPerSecond += UpdateInfoInUpgradeMoneyPerSecondButton;
        _levelUpgrade.onChangeMoney += UpdateCoinCountText;
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        UpdateUpgradeCardInfo();
    }

    private void OnEnable()
    {
        _moneyPerSeconsUpgrade.onClick.AddListener(
            () => _levelUpgrade.UpgradeMoneyPerSecond());

        UpdateInfoInUpgradeMoneyPerSecondButton();
    }

    private void OnDisable()
    {
        _moneyPerSeconsUpgrade.onClick.RemoveAllListeners();
    }

    protected override void Initialized()
    {
        _unitUpgradesCards = new UnitCard[_playerUnits.Length];

        if(_unitUpgradesContainer.childCount > 0)
        {
            for (int i = 0; i < _unitUpgradesContainer.childCount; i++)
                Destroy(_unitUpgradesContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < _playerUnits.Length; i++)
        {
            UnitCard card = Instantiate(_unitUpgradesCardTemplate);
            _DIContainer.Inject(card);
            card.transform.SetParent(_unitUpgradesContainer, false);
            _unitUpgradesCards[i] = card;
        }

        UpdateUpgradeCardInfo();

        _moneyPerSeconsUpgrade.GetComponentInChildren<TMP_Text>().text
            = _levelUpgrade.CurrentGameModifier.Cost.ToString();
    }

    private void UpdateUpgradeCardInfo()
    {
        for (int i = 0; i < _unitUpgradesCards.Length; i++)
        {
            _unitUpgradesCards[i].UpdateInfo(_playerUnits[i]);
        }
    }

    private void UpdateInfoInUpgradeMoneyPerSecondButton()
    {
        Debug.Log(_levelUpgrade.CanUpgradeMoneyPerSecond());

        if (_levelUpgrade.CanUpgradeMoneyPerSecond())
        {
            _moneyPerSeconsUpgrade.GetComponentInChildren<TMP_Text>().text
            = _levelUpgrade.CurrentGameModifier.Cost.ToString();
        }
        else
        {
            _moneyPerSeconsUpgrade.GetComponentInChildren<TMP_Text>().text =
                "MAX";
        }
    }

    private void UpdateCoinCountText(int value) => _coinCountText.text = value.ToString();

    public void SetLevelSettings(LevelSetting levelSettings)
    {
        
    }

    public void SetEraSettings(LevelSetting levelSettings)
    {
        _playerUnits = levelSettings.PlayerUnits;
        Initialized();
    }
}
