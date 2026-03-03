using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FortunaMenu : Menu
{
    private enum PrizeType
    {
        DonatCoin,
        Coin
    }

    [System.Serializable]
    private struct Prize
    {
        public PrizeType PrizeType;
        public int Value;
    }

    [Header("Main Settings")]
    [SerializeField] private float _spinDuration;
    [SerializeField] private float _minSpins;

    [Header("Prizes Settings")]
    [SerializeField] private List<Prize> _prizes;

    [Header("UI Links")]
    [SerializeField] private Transform _wheel;
    [SerializeField] private Button _spinButton;

    private bool _isSpinning = false;
    private float _currentAngle = 0f;

    private void OnEnable()
    {
        _spinButton.onClick.AddListener(OnSpinButton);
    }

    private void OnDisable()
    {
        _spinButton.onClick.RemoveListener(OnSpinButton);
    }

    protected override void Initialized()
    {
        CloseMenu();
    }

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    private void OnSpinButton()
    {
        if (_isSpinning == true) return;

        StartCoroutine(SpinWheel());
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

        Debug.Log($"Dropped prizes index {index}");
    }
}