using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TutorialView : MonoBehaviour
{
    [Inject] private LocalizationSelector _localizeSelector;

    [Header("<color=green>Text Animation Settings</color>")]
    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private Ease _ease = Ease.OutQuad;
    [SerializeField] private bool _enableRichText = true;
    [SerializeField] private float _soundInterval;

    [Header("<color=green>Talking Animation Settings</color>")]
    [SerializeField] private float _baseDuration = 0.35f; 
    [SerializeField] private float _scaleAmount = 0.05f;
    [SerializeField] private float _bobAmount = 3f;       
    [SerializeField] private float _tiltAmount = 2f;      
    [SerializeField] private Ease _easeTalking = Ease.InOutSine;
    [SerializeField] private bool _randomizePhase = true;

    [Space(5f)]

    [Header("<color=yellow>UI Links</color>")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Image _headImage;

    private float _lastPlaySoundTime;

    private Tween _currentTextTween;

    private RectTransform _rectTransform;
    private readonly List<Tween> _activeTweens = new();
    private bool _isTalking;

    private Vector3 _originalScaleHead;
    private Vector2 _originalPosHead;
    private Vector3 _originalRotHead;

    private void Awake()
    {
        _rectTransform = _headImage.rectTransform;
        _originalPosHead = _rectTransform.anchoredPosition;
        _originalRotHead = _rectTransform.localEulerAngles;
        _originalScaleHead = _rectTransform.localScale;
    }

    public void SetTutorialInfo(TutorialInfo info)
    {
        PlayAnimationHead();
        PlayTextAnimatio(info.Description.GetText(_localizeSelector.CurrentLanguage));
        info.Arrows.ForEach(i => i.enabled = true);
    }

    public void SetTutorialTitle(string title)
    {
        _titleText.text = title;
        _canvasGroup.Show();
    }

    public void HideTutorialCanvas() => _canvasGroup.Hide();

    private void PlayTextAnimatio(string text)
    {
        _currentTextTween?.Kill();
        _descriptionText.maxVisibleCharacters = 0;
        _descriptionText.text = text;

        _currentTextTween = 
            DOTween.To
            (
                () => _descriptionText.maxVisibleCharacters,
                x => _descriptionText.maxVisibleCharacters = (int)x,
                text.Length, _duration
            )
            .SetEase(_ease)
            .SetUpdate(true)
            .OnUpdate(PlayTalkingSound)
            .OnComplete(() => 
            {
                _descriptionText.maxVisibleCharacters = text.Length;
                StopAnimatioHead();
            });
    }

    private void PlayTalkingSound()
    {
        if (Time.unscaledTime - _lastPlaySoundTime >= _soundInterval)
        {
            AudioManager.PlaySound("Talking");
            _lastPlaySoundTime = Time.unscaledTime;
        }
    }

    private void PlayAnimationHead()
    {
        if (_isTalking == true) return;

        _isTalking = true;
        StopAnimatioHead();

        float seed = _randomizePhase ? Random.Range(0f, _baseDuration) : 0f;

        var scaleTween = _rectTransform.DOScaleY(
            _originalScaleHead.y + _scaleAmount,
            _baseDuration * 0.5f)
            .SetEase(_easeTalking)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(seed)
            .SetUpdate(true)
            .SetRecyclable(true);

        _activeTweens.Add(scaleTween);

        var bobTween = _rectTransform.DOAnchorPos(
            new Vector2(_originalPosHead.x, _originalPosHead.y + _bobAmount), 
                _baseDuration * 0.7f)
            .SetEase(_easeTalking)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(seed + 0.1f)
            .SetUpdate(true)
            .SetRecyclable(true);

        _activeTweens.Add(bobTween);

        var tiltTween = transform.DOLocalRotate(new Vector3(0, 0, _tiltAmount), _baseDuration * 0.9f)
            .SetEase(_easeTalking)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(seed + 0.2f)
            .SetUpdate(true)
            .SetRecyclable(true);
        _activeTweens.Add(tiltTween);
    }

    private void StopAnimatioHead()
    {
        foreach (var t in _activeTweens) t.Kill();

        _activeTweens.Clear();
        
        _rectTransform.localScale = _originalScaleHead;
        _rectTransform.anchoredPosition = _originalPosHead;
        
        _headImage.transform.localEulerAngles = _originalRotHead;

        _isTalking = false;
    }
}
