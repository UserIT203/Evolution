using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Events;

public class GameView : MonoBehaviour
{
    [Inject] private DiContainer _diContainer;

    [SerializeField] private TMP_Text _currentMeatText;
    [SerializeField] private Image _fillImageProgressMoney;

    [Header("Anility UI Links")]
    [SerializeField] private Image _abilityIcon;
    [SerializeField] private Button _abilityButton;

    [Header("Unit Card")]
    [SerializeField] private UnitGameView _unitCardTemplate;
    [SerializeField] private Transform _unitCardContainer;

    public void SetCurrentMoneyText(int currentMoney)
    {
        _currentMeatText.text = currentMoney.ToString();
    }

    public void ChangeImageFill(float max, float value)
    {
        _fillImageProgressMoney.fillAmount = value / max;
    }

    public void CreateUnitCard(UnitBase unit, int index)
    {
        UnitGameView card = Instantiate(_unitCardTemplate, _unitCardContainer);
        card.Initialized(unit, index);
        _diContainer.Inject(card);
    }

    public void RestartUI()
    {
        for (int i = 0; i < _unitCardContainer.childCount; i++)
        {
            Destroy(_unitCardContainer.GetChild(i).gameObject);
        }
    }

    public void UpdateAbilityButton(Ability ability)
    {
        _abilityIcon.enabled = true;
        _abilityIcon.sprite = ability.Sprite;
    }

    public void InitializedAbilityButton(UnityAction operation)
    {
        _abilityButton.onClick.RemoveAllListeners();
        _abilityButton.onClick.AddListener(operation);
    }
}
