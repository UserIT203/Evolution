using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UnitGameView : MonoBehaviour
{
    [Inject] private GameManager _gameManager;

    [SerializeField] private TMP_Text _unitCostText;

    private Button _button;
    private UnitBase _unit;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(BuyUnit);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(BuyUnit);
    }

    private void Update()
    {
        if (_gameManager.CurrentMoney >= _unit.UnitConfig.Cost)
        {
            _button.enabled = true;
        }
        else
        {
            _button.enabled = false;
        }
    }

    private void BuyUnit()
    {
        _gameManager.SpawnUnit(_unit);
    }

    private void SetUI()
    {
        _unitCostText.text = _unit.UnitConfig.Cost.ToString();
    }

    public void Initialized(UnitBase unit)
    {
        Debug.Log("Init card");

        _unit = unit;
        SetUI();
    }
}
