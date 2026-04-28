using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization.Components;


[RequireComponent(typeof(CanvasGroup))]
public class WaveNotificationMenu : MonoBehaviour
{
    private const string LOCALIZE_LABEL = "MenuLabels";

    [Header("Animation Settings")]
    [SerializeField, Tooltip("Длительность появления (сек)")] private float _fadeInDuration = 0.4f;
    [SerializeField, Tooltip("Время отображения на экране (сек)")] private float _stayDuration = 2.0f;
    [SerializeField, Tooltip("Длительность исчезновения (сек)")] private float _fadeOutDuration = 0.4f;

    [SerializeField] private Vector3 _initialScale = new Vector3(0.85f, 0.85f, 1f);
    [SerializeField] private Vector3 _targetScale = Vector3.one;
    [SerializeField] private Ease _inEase = Ease.OutBack;
    [SerializeField] private Ease _outEase = Ease.InBack;

    [Header("<color=green>UI Links</color>")]
    [SerializeField] private LocalizeStringEvent _labelStringEvent;

    private CanvasGroup _canvasGroup;
    private Sequence _activeSequence;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _canvasGroup.Hide();
        
    }

    private void OnDestroy()
    {
        _activeSequence?.Kill();
    }

    public void ShowNotification(int currentWaveIndex)
    {
        _activeSequence?.Kill();

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0f;
        transform.localScale = _initialScale;

        if (_labelStringEvent.StringReference.Arguments == null)
            _labelStringEvent.StringReference.Arguments = new object[1];

        _labelStringEvent.StringReference.Arguments[0] = currentWaveIndex + 1;
        _labelStringEvent.RefreshString();

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        _activeSequence = DOTween.Sequence();

        _activeSequence.Append(_canvasGroup.DOFade(1f, _fadeInDuration));
        _activeSequence.Join(transform.DOScale(_targetScale, _fadeInDuration).SetEase(_inEase));

        _activeSequence.AppendInterval(_stayDuration);

        _activeSequence.Append(_canvasGroup.DOFade(0f, _fadeOutDuration));
        _activeSequence.Join(transform.DOScale(_initialScale, _fadeOutDuration).SetEase(_outEase));

        _activeSequence.OnComplete(() =>
        {
            _canvasGroup.Hide();
        });

        _activeSequence.Play();
    }
}
