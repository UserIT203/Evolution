using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UnitCard : MonoBehaviour
{
    [Inject] private LevelUpgrade _levelUpgrade;

    [Header("Unlock Objects")]
    [SerializeField] private GameObject _unlockObjects;
    [SerializeField] private TMP_Text _unitNameText;
    [SerializeField] private TMP_Text _unitDamageText;
    [SerializeField] private TMP_Text _unitHealthText;

    [Header("Lock Objects")]
    [SerializeField] private GameObject _lockObjects;
    [SerializeField] private TMP_Text _unitCostText;
    [SerializeField] private Button _unlockButton;

    private UnitStatsConfig _unitConfig;

    private void Awake()
    {
        _unlockButton.onClick.AddListener(OnUnlockUnit);
    }

    public void UpdateInfo(UnitBase unit)
    {
        if(unit.UnitConfig.IsUnlock == false)
        {
            _unlockObjects.SetActive(false);
            _lockObjects.SetActive(true);
        }
        else
        {
            _unlockObjects.SetActive(true);
            _lockObjects.SetActive(false);
        }

        _unitNameText.text = unit.UnitConfig.UnitName;
        _unitDamageText.text = unit.UnitConfig.Damage.ToString();
        _unitHealthText.text = unit.UnitConfig.Maxhealth.ToString();

        _unitCostText.text = unit.UnitConfig.UnlockCosts.ToString();

        _unitConfig = unit.UnitConfig;
    }

    private void OnUnlockUnit()
    {
        if (_levelUpgrade.TryRemoveCoins(_unitConfig.UnlockCosts))
        {
            _unlockObjects.SetActive(true);
            _lockObjects.SetActive(false);
            _unitConfig.UnlockUnit();
        }
    }
}
