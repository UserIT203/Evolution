using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
using Zenject;
using System.Diagnostics;

public class EraCompletedMenu : Menu
{
    private const float CHANGE_VALUE_DELAY = 0.25f;

    [SerializeField] private int _maxValue;
    [SerializeField] private int _minValue;

    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private Button _priceButton;

    private LevelManager _levelManager;
    private GlobalManager _globalManager;

    private int _currentPrice;
    private float _timer;
    private bool _isOpen = false;

    [Inject]
    public void Construct(LevelManager levelManager, GlobalManager globalManager)
    {
        _levelManager = levelManager;
        _globalManager = globalManager;

        _levelManager.onEraCompleted += OpenMenu;
    }

    private void OnEnable()
    {
        _priceButton.onClick.AddListener(GetPrice);   
    }

    private void LateUpdate()
    {
        if (_isOpen == false) return;

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            int randomValue = UnityEngine.Random.Range(_minValue, _maxValue);

            _fillImage.fillAmount = Mathf.MoveTowards(
                _fillImage.fillAmount,
                (float)randomValue / (float)_maxValue,
                10f * Time.deltaTime
                );

            _coinText.text = randomValue.ToString();
            _currentPrice = randomValue;

            _timer = CHANGE_VALUE_DELAY;
        }
    }

    private void OnDestroy()
    {
        _levelManager.onEraCompleted -= OpenMenu;
        _priceButton.onClick.RemoveListener(GetPrice);
    }

    protected override void Initialized()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void GetPrice()
    {
        _globalManager.GemCount = _currentPrice;
        CloseMenu();
    }

    public override void OpenMenu()
    {
        _isOpen = true;

        _canvasGroup.enabled = true;
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public override void CloseMenu()
    {
        _isOpen = false;

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
