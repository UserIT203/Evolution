using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrizeView : MonoBehaviour 
{
    [SerializeField] private Image _prizeIcon;
    [SerializeField] private TMP_Text _prizeValueText;
    [SerializeField] private Image _slotIcon;

    public void Initialized(Color slotColot, Sprite prizeIcon, int prizeValue)
    {
        _prizeIcon.sprite = prizeIcon;
        _slotIcon.color = slotColot;
        _prizeValueText.text = prizeValue.ToString();
    }
}
