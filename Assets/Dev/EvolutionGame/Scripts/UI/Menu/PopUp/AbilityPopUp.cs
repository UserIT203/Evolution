using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class AbilityPopUp : PopUp
{
    [Header("Ability UI Links")]
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Button _equipmentButton;


    private void Start()
    {
        _equipmentButton.onClick.AddListener(EquipAbility);
    }


    protected override void FillUI()
    {
        base.FillUI();

        _description.text = _cardItem.Description.GetText(_localizationSelector.CurrentLanguage);
    }

    private void EquipAbility()
    {
        AudioManager.PlaySound("DroppedCard");
        AbilityManager abilityManager = _collected as AbilityManager;
        abilityManager.ChangeAbility(_cardItem.CardID);
    }

    protected override void UpdateLocaleText()
    {
        if(_cardItem != null)
            _description.text = _cardItem.Description.GetText(_localizationSelector.CurrentLanguage);
    }
}
