using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CardPop : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] private TMP_Text _cardName;
    [SerializeField] private Slider _levelSlider;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Transform _modifierContainer;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _closeButton;

    [Header("Main Links")]
    [SerializeField] private ModifierView _modifierViewPrefab;

    private ICollectedCard _collectManager;
    private CardItem _card;

    private void OnEnable()
    {
        _upgradeButton.onClick.AddListener(UpgradeCard);
        _closeButton.onClick.AddListener(ClosePopUp);
    }

    private void OnDisable()
    {
        _upgradeButton.onClick.RemoveListener(UpgradeCard);
        _closeButton.onClick.RemoveListener(ClosePopUp);
    }

    public void OpenPopUp(CardItem card, ICollectedCard collectManager)
    {
        gameObject.SetActive(true);
        _card = card;
        _collectManager = collectManager;

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        _cardName.text = _card.CardName;
        _descriptionText.text = _card.Description;

        _levelSlider.value = _collectManager.GetCollectedCards(_card.CardID) -
            _collectManager.GetCardsNeededForNextLevel(_card.CardID);
            

        if(_modifierContainer.childCount > 0)
        {
            for (int i = 0; i < _modifierContainer.childCount; i++)
            {
                Destroy(_modifierContainer.GetChild(i).gameObject);
            }
        }

        if(_card is UnitUpradeCardConfig upgrade)
        {
            _equipButton.gameObject.SetActive(false);

            if(upgrade.GetScaledModifier(
                upgrade.BaseDamageModifier,
                _collectManager.GetLevel(upgrade.CardID)).ModifierValue != 0)
            {
                ModifierView modifierView = Instantiate(_modifierViewPrefab);
                modifierView.transform.SetParent(_modifierContainer, false);

                modifierView.Initialized(
                    ModifierType.Damage,
                    upgrade.GetScaledModifier(upgrade.BaseDamageModifier, _collectManager.GetLevel(upgrade.CardID)).ModifierValue,
                    upgrade.GetScaledModifier(upgrade.BaseDamageModifier, _collectManager.GetLevel(upgrade.CardID) + 2).ModifierValue);
            }

            if (upgrade.GetScaledModifier(
                upgrade.BaseSpeedModifier,
                _collectManager.GetLevel(upgrade.CardID)).ModifierValue != 0)
            {
                ModifierView modifierView = Instantiate(_modifierViewPrefab);
                modifierView.transform.SetParent(_modifierContainer, false);

                modifierView.Initialized(
                    ModifierType.Damage,
                    upgrade.GetScaledModifier(upgrade.BaseSpeedModifier, _collectManager.GetLevel(upgrade.CardID)).ModifierValue,
                    upgrade.GetScaledModifier(upgrade.BaseSpeedModifier, _collectManager.GetLevel(upgrade.CardID) + 2).ModifierValue);
            }

            if (upgrade.GetScaledModifier(
                upgrade.BaseHealthModifier,
                _collectManager.GetLevel(upgrade.CardID)).ModifierValue != 0)
            {
                ModifierView modifierView = Instantiate(_modifierViewPrefab);
                modifierView.transform.SetParent(_modifierContainer, false);

                modifierView.Initialized(
                    ModifierType.Damage,
                    upgrade.GetScaledModifier(upgrade.BaseHealthModifier, _collectManager.GetLevel(upgrade.CardID)).ModifierValue,
                    upgrade.GetScaledModifier(upgrade.BaseHealthModifier, _collectManager.GetLevel(upgrade.CardID) + 2).ModifierValue);
            }
        }
        else if (_card is Ability ability)
        {
            _equipButton.gameObject.SetActive(true);
            _equipButton.onClick.RemoveAllListeners();

            AbilityManager abilityManager = _collectManager as AbilityManager;
            _equipButton.onClick.AddListener(() => abilityManager.ChangeAbility(ability.CardID));
        }

        _upgradeButton.enabled =
                _collectManager.GetCardsNeededForNextLevel(_card.CardID)
                <= _collectManager.GetCollectedCards(_card.CardID) ? true : false;
    }

    private void ClosePopUp()
    {
        gameObject.SetActive(false);
    }

    private void UpgradeCard()
    {
        _collectManager.TryUpgrade(_card.CardID);
    }
}
