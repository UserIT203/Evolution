using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifierPopUp : PopUp
{
    [System.Serializable]
    public struct ModifierColor
    {
        public ModifierType Type;
        public Color Color;
    }

    [Header("Modifier PopUp Links")]
    [SerializeField] private Transform _modifierContainer;

    [Header("Modifier View Settings")]
    [SerializeField] private ModifierView _modifierViewPrefab;
    [SerializeField] private List<ModifierColor> _modifiersColor;

    private List<ModifierView> _modifiers = new List<ModifierView>();

    public override void Exit()
    {
        base.Exit();

        foreach (var modifier in _modifiers)
        {
            Destroy(modifier.gameObject);
        }

        _modifiers.Clear();
    }

    protected override void FillUI()
    {
        base.FillUI();

        UnitUpradeCardConfig upgradeConfig = _cardItem as UnitUpradeCardConfig;

        AddModifierView(upgradeConfig);
    }

    private void AddModifierView(UnitUpradeCardConfig upgradeConfig)
    {
        if (_modifiers.Count > 0)
        {
            foreach (var modifier in _modifiers)
            {
                Destroy(modifier.gameObject);
            }

            _modifiers.Clear();
        }

        if (upgradeConfig.BaseDamageModifier.ModifierValue != 0)
        {
            ModifierView modifierView = Instantiate(_modifierViewPrefab, _modifierContainer, false);
            Color color = _modifiersColor.Find(m => m.Type == ModifierType.Damage).Color;
            modifierView.Initialized(
                upgradeConfig.GetScaledModifier(
                    upgradeConfig.BaseDamageModifier, 
                    _collected.GetLevel(upgradeConfig.CardID) + 1
                    )
                .ModifierValue, 
                color);

            _modifiers.Add(modifierView);
        }

        if (upgradeConfig.BaseHealthModifier.ModifierValue != 0)
        {
            ModifierView modifierView = Instantiate(_modifierViewPrefab, _modifierContainer, false);
            Color color = _modifiersColor.Find(m => m.Type == ModifierType.Health).Color;
            modifierView.Initialized(
                upgradeConfig.GetScaledModifier(
                    upgradeConfig.BaseHealthModifier,
                    _collected.GetLevel(upgradeConfig.CardID) + 1
                    )
                .ModifierValue,
                color);

            _modifiers.Add(modifierView);
        }

        if (upgradeConfig.BaseSpeedModifier.ModifierValue != 0)
        {
            ModifierView modifierView = Instantiate(_modifierViewPrefab, _modifierContainer, false);
            Color color = _modifiersColor.Find(m => m.Type == ModifierType.Speed).Color;
            modifierView.Initialized(
                upgradeConfig.GetScaledModifier(
                    upgradeConfig.BaseSpeedModifier,
                    _collected.GetLevel(upgradeConfig.CardID) + 1
                    )
                .ModifierValue,
                color);

            _modifiers.Add(modifierView);
        }
    }
}
