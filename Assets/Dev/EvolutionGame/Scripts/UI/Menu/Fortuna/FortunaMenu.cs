using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class FortunaMenu : Menu
{
    private const float SPIN_DURATION_HOURS = 2f;

    private enum PrizeType
    {
        DonatCoin,
        Coin
    }

    [System.Serializable]
    private struct Prize
    {
        public PrizeType PrizeType;
        public Color Color;
        public int Value;
    }

    [Header("Prize Icon")]
    [SerializeField] private Sprite _coinImage;
    [SerializeField] private Sprite _gemImage;

    [Header("Main Settings")]
    [SerializeField] private float _spinDuration;
    [SerializeField] private float _minSpins;

    [Header("Prizes Settings")]
    [SerializeField] private List<Prize> _prizes;

    [Header("UI Links")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Transform _prizesContainer;
    [SerializeField] private PrizeView _prizeViewPrefab;
    [SerializeField] private Transform _wheel;
    [SerializeField] private Button _spinButton;

    [Header("<color=yellow>PopUp UI Links</color>")]
    [SerializeField] private Image _popUpBackground;
    [SerializeField] private CanvasGroup _prizePopUp;
    [SerializeField] private Image _prizeIcon;
    [SerializeField] private TMP_Text _prizeValue;

    [Header("Animation Settings")]
    [Tooltip("Длительность анимации появления")]
    [SerializeField] private float _showDuration = 0.5f;
    [Tooltip("Длительность анимации скрытия")]
    [SerializeField] private float _hideDuration = 0.4f;
    [Tooltip("Время, которое окно остаётся видимым")]
    [SerializeField] private float _stayVisibleTime = 2.0f;

    private GlobalManager _globalManager;
    private LevelUpgrade _levelUpgrade;

    private DateTime? _lastSpinTime
    {
        get
        {
            string data = PlayerPrefs.GetString("lastSpinTime", null);

            if (string.IsNullOrEmpty(data) == false)
                return DateTime.Parse(data);

            return null;
        }

        set
        {
            if (value != null)
                PlayerPrefs.SetString("lastSpinTime", value.ToString());
            else
                PlayerPrefs.DeleteKey("lastSpinTime");
        }
    }

    private float _spinTimer;
    private bool _isSpinning = false;
    private float _currentAngle = 0f;

    private Sequence _showSeq;


    [Inject]
    public void Construnt(GlobalManager globalManager, LevelUpgrade levelUpgrade)
    {
        _globalManager = globalManager;
        _levelUpgrade = levelUpgrade;
    }

    private void OnEnable()
    {
        _spinButton.onClick.AddListener(OnSpinButton);
        _closeButton.onClick.AddListener(CloseMenu);
    }

    private void OnDisable()
    {
        _spinButton.onClick.RemoveListener(OnSpinButton);
        _closeButton.onClick.RemoveListener(CloseMenu);
    }

    private void OnDestroy()
    {
        _showSeq?.Kill();
    }

    private void Update()
    {
        if(Time.time - _spinTimer > 3)
        {
            _spinTimer = Time.time;
            UpdateTime();
        }
    }

    public override void Initialized()
    {
        UpdateTime();

        CloseMenu();

        if(_prizes.Count > 0)
        {
            foreach (var slot in _prizes)
            {
                PrizeView prizeView = Instantiate(_prizeViewPrefab, _prizesContainer, false);
                Sprite icon = slot.PrizeType == PrizeType.Coin ? _coinImage : _gemImage;
                prizeView.Initialized(slot.Color, icon, slot.Value);
            }
        }
    }

    public override void CloseMenu()
    {
        if (IsClosed == true) return;

        _canvasGroup.Hide();

        IsClosed = true;

        _showSeq?.Kill();

        MenuManager?.OpenMenu(0);
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();
        _prizePopUp.Hide();

        IsClosed = false;
    }

    private void OnSpinButton()
    {
        if (_isSpinning == true || TrySpeen() == false) return;

        _lastSpinTime = DateTime.UtcNow;

        StartCoroutine(SpinWheel());
    }

    private bool TrySpeen()
    {
        if(_lastSpinTime == null) return true;

        var timeSpan = DateTime.UtcNow - _lastSpinTime.Value;

        if (timeSpan.Hours >= SPIN_DURATION_HOURS) return true;

        return false;
    }

    private void UpdateTime()
    {
        if(_lastSpinTime == null)
        {
            _timeText.text = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
            return;
        }

        var timeRemaing = _lastSpinTime.Value.AddHours(SPIN_DURATION_HOURS) - DateTime.UtcNow;
        var time = timeRemaing > TimeSpan.Zero ? timeRemaing : TimeSpan.Zero;
        _timeText.text = time.ToString(@"hh\:mm\:ss");
    }

    private IEnumerator SpinWheel()
    {
        _isSpinning = true;

        float randomStopAngle = UnityEngine.Random.Range(0f, 360f);

        float targetAngle  = _currentAngle + (360f * _minSpins) + randomStopAngle;

        float startTime = Time.time;
        float startAngle = _currentAngle;

        while(Time.time - startTime < _spinDuration)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / _spinDuration;

            t = 1 - Mathf.Pow(1 - t, 3);

            _currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            _wheel.eulerAngles = new Vector3(0f, 0f, _currentAngle);

            yield return null;
        }

        _currentAngle = targetAngle;
        _wheel.eulerAngles = new Vector3(0F, 0F, _currentAngle);

        _isSpinning = false;

        DetermineWinner();
    }

    private void DetermineWinner()
    {
        float normalizedAngle = _currentAngle % 360;

        float offset = 270f;
        float adjustedAngle = (normalizedAngle + offset) % 360;

        float segmentAngle = 360 / _prizes.Count;

        int index = _prizes.Count - 1 - Mathf.FloorToInt(adjustedAngle / segmentAngle);

        index = Mathf.Clamp(index, 0, _prizes.Count - 1);

        Prize prize = _prizes[index];

        AudioManager.PlaySound("SpinWheel");

        Sprite sprite = null;

        if (prize.PrizeType == PrizeType.Coin)
        {
            _levelUpgrade.AddCoin(prize.Value);
            sprite = _coinImage;
        }         
        else if (prize.PrizeType == PrizeType.DonatCoin)
        {
            _globalManager.GemCount = prize.Value;
            sprite = _gemImage;
        }
            
        OpenPrizePopUp(sprite, prize.Value);

        Debug.Log($"Dropped prizes index {index}");
    }

    private void OpenPrizePopUp(Sprite icon, int value)
    {
        _prizeValue.text= value.ToString();
        _prizeIcon.sprite = icon;

        _prizePopUp.alpha = 0f;
        _popUpBackground.rectTransform.localScale = Vector3.one * 0.3f;
        _popUpBackground.rectTransform.anchoredPosition = Vector2.up * 60f;

        _showSeq = DOTween.Sequence().SetId("ShowWinPopup");

        _showSeq.Append(_prizePopUp.DOFade(1f, _showDuration * 0.6f));

        // 2. Масштаб с отскоком
        _showSeq.Join(_popUpBackground.rectTransform.DOScale(1f, _showDuration).SetEase(Ease.OutBack));

        // 3. Сдвиг в центр
        _showSeq.Join(_popUpBackground.rectTransform.DOLocalMoveY(0f, _showDuration * 0.8f).SetEase(Ease.OutCubic));

        _showSeq.Join(_prizeIcon.transform.DOScale(1.15f, _showDuration * 0.5f)
               .SetEase(Ease.OutQuad)
               .OnComplete(() => _prizeIcon.transform.DOScale(1f, _showDuration * 0.3f).SetEase(Ease.InOutQuad)));

        _showSeq.AppendInterval(_stayVisibleTime);

        _showSeq.Append(_prizePopUp.DOFade(0f, _hideDuration));
        _showSeq.Join(_popUpBackground.rectTransform.DOScale(0.3f, _hideDuration).SetEase(Ease.InBack));
    }
}