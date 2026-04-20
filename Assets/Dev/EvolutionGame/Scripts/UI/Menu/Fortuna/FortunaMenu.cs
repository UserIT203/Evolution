using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using System;

public class FortunaMenu : Menu
{
    private const float SPIN_DURATION_HOURS = 50f;

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

        MenuManager?.OpenMenu(0);
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();

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

        if (timeSpan.Seconds >= SPIN_DURATION_HOURS) return true;

        return false;
    }

    private void UpdateTime()
    {
        if(_lastSpinTime == null)
        {
            _timeText.text = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
            return;
        }

        var timeRemaing = _lastSpinTime.Value.AddSeconds(SPIN_DURATION_HOURS) - DateTime.UtcNow;
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

        if (prize.PrizeType == PrizeType.Coin)
            _levelUpgrade.AddCoin(prize.Value);
        else if (prize.PrizeType == PrizeType.DonatCoin)
            _globalManager.GemCount = prize.Value;

        Debug.Log($"Dropped prizes index {index}");
    }
}