using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinViewUI : MonoBehaviour
{
    [Header("<color=green><b>UI Links</b></color>")]
    [SerializeField] private TMP_Text _coinCountText;
    [Space(10f)]
    [Header("<color=green><b>Settings</b></color>")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _popScale;
    [SerializeField] private float _floatDistance;

    private Camera _mainCamera;
    private float _currentCoinValue = 0;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _coinCountText.enabled = false;
        transform.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
        transform.LookAt(_mainCamera.transform);
    }

    public void PlayAnimation(int amount)
    {
        KillTween();

        _currentCoinValue = 0;
        _coinCountText.enabled = true;
        transform.localScale = Vector3.one; 
        _coinCountText.text = "+" + 0;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(_popScale, 0.2f).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad));


        seq.Join(DOTween.To(() => _currentCoinValue, x => {
            _currentCoinValue = x;
            _coinCountText.text = "+" + Mathf.FloorToInt(_currentCoinValue);
        }, amount, _animationDuration).SetEase(Ease.OutQuad));

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * _floatDistance;

        seq.Join(transform.DOLocalMove(endPos, _animationDuration).SetEase(Ease.OutQuad));
        seq.Join(_coinCountText.DOFade(0f, _animationDuration).SetEase(Ease.InQuad));


        seq.OnComplete(() => {
            _coinCountText.enabled = false;
            transform.localPosition = startPos;
            transform.localScale = Vector3.one;
            _coinCountText.color = 
                new Color(
                    _coinCountText.color.r, 
                    _coinCountText.color.g, 
                    _coinCountText.color.b, 
                    1f);
        });
    }

    private void OnDestroy()
    {
        KillTween();
    }

    private void KillTween()
    {
        DOTween.Kill(transform);
        DOTween.Kill(_coinCountText);
    }
}
