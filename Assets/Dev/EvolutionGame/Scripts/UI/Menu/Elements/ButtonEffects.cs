using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class ButtonEffects : MonoBehaviour
{
    [SerializeField] private string _soundName = "Click";

    private Button _button;

    private void OnEnable()
    {
        _button.onClick.AddListener(() => AudioManager.PlaySound(_soundName));
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(() => AudioManager.PlaySound(_soundName));
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
}
