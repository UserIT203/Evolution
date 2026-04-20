using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ModifierView : MonoBehaviour
{
    [SerializeField] private TMP_Text _modifierValue;

    private Image _background;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    public void Initialized(float modifierValue, Color color)
    {
        _modifierValue.text = modifierValue.ToString();
        _background.color = color;
    }
}
