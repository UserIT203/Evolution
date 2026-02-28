using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Image))]
public class ModifierView : MonoBehaviour
{
    [SerializeField] private ModifierViewSettings[] _settings;

    [Header("UI Links")]
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Image _icon;

    private Image _background;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    public void Initialized(ModifierType type, float currentValue, float nextLevelValue)
    {
        ModifierViewSettings setting = _settings.First(m => m.ModifierType == type);

        _background.color = setting.BackgroundColor;
        _icon.sprite = setting.Icon;

        _label.text = string.Format($"{currentValue} -> {nextLevelValue}");
    }
}

[System.Serializable]
public struct ModifierViewSettings
{
    public ModifierType ModifierType;
    public Color BackgroundColor;
    public Sprite Icon;
}

public enum ModifierType
{
    Health,
    Damage,
    Speed
}
