using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsView : MonoBehaviour
{
    [SerializeField] private Image _healthBarSlider;
    [SerializeField] private Image _armorBarSlider;
    [SerializeField] private TMP_Text _victimText;

    private void Awake()
    {
        _healthBarSlider.enabled = false;
        _armorBarSlider.enabled = false;
    }

    public void ChangeHealthBarValue(float maxValue, float value)
    {
        if(_healthBarSlider.enabled == false) _healthBarSlider.enabled = true;

        _healthBarSlider.fillAmount = value / maxValue;
    }

    public void ChangeArmorBarValue(float maxValue, float value)
    {
        if (_armorBarSlider.enabled == false) _armorBarSlider.enabled = true;

        _armorBarSlider.fillAmount = value / maxValue;
    }

    public void SetVictimText(string text) => _victimText.text = text;
}
