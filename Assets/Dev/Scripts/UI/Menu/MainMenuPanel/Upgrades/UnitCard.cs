using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UnitCard : MonoBehaviour
{
    [Inject] private LevelUpgrade _levelUpgrade;

    [SerializeField] private Image _unitIcon;

    [Header("Unlock Objects")]
    [SerializeField] private GameObject _unlockObjects;
    [SerializeField] private TMP_Text _unitNameText;
    [SerializeField] private TMP_Text _unitDamageText;
    [SerializeField] private TMP_Text _unitHealthText;

    [Header("Lock Objects")]
    [SerializeField] private Sprite _unlockIcon;
    [SerializeField] private GameObject _lockObjects;
    [SerializeField] private TMP_Text _unitCostText;
    [SerializeField] private Button _unlockButton;

    private UnitInfo _unitInfo;

    private void Awake()
    {
        _unlockButton.onClick.AddListener(OnUnlockUnit);
    }

    public void UpdateInfo(UnitInfo unitInfo)
    {
        if (unitInfo.IsUnlock == false)
        {
            _unlockObjects.SetActive(false);
            _lockObjects.SetActive(true);
            _unitIcon.sprite = _unlockIcon;
        }
        else
        {
            _unlockObjects.SetActive(true);
            _lockObjects.SetActive(false);
            _unitIcon.sprite = unitInfo.Unit.UnitConfig.Icon;
        }

        _unitNameText.text = unitInfo.Unit.UnitConfig.UnitName;
        _unitDamageText.text = unitInfo.Unit.UnitConfig.Damage.ToString();
        _unitHealthText.text = unitInfo.Unit.UnitConfig.Maxhealth.ToString();

        _unitCostText.text = unitInfo.Unit.UnitConfig.UnlockCosts.ToString();

        _unitInfo = unitInfo;
        
    }

    private void OnUnlockUnit()
    {
        if (_levelUpgrade.TryRemoveCoins(_unitInfo.Unit.UnitConfig.UnlockCosts))
        {
            _unitIcon.enabled = true;
            _unitIcon.sprite = _unitInfo.Unit.UnitConfig.Icon;
            _unlockObjects.SetActive(true);
            _lockObjects.SetActive(false);
            _unitInfo.IsUnlock = true;

            AudioManager.PlaySound("Upgrade");
        }
    }
}
