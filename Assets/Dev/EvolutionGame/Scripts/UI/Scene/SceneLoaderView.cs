using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SceneLoaderView : MonoBehaviour
{
    [Header("<color=green>Cloud Animation Settings</color>")]
    [SerializeField] private Vector3 _moveOffset;
    [SerializeField] private float _moveDuration = 5f;
    [SerializeField] private Ease _moveEase = Ease.InOutSine;
    [SerializeField] private float _fadeMin = 0.7f;
    [SerializeField] private float _fadeMax = 1f;
    [SerializeField] private float _fadeDuration = 3f;

    [Header("UI Links")]
    [SerializeField] private Image _fillLoaderBar;
    [SerializeField] private Image _cloudIcon;

    private Tween _moveTween;
    private Tween _fadeTween;

    private SceneLoader _loader;

    [Inject] 
    public void Construct(SceneLoader loader)
    {
        _loader = loader;

        _loader.onStartLoadScene += Open;
        _loader.onEndLoadScene += Close;
        _loader.onLoadSceneProgress += ChangeLoaderBarValue;
    }

    private void Awake()
    {
        Open();
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        _loader.onStartLoadScene -= Open;
        _loader.onEndLoadScene -= Close;
        _loader.onLoadSceneProgress -= ChangeLoaderBarValue;
    }

    public void ChangeLoaderBarValue(float value)
    {
        _fillLoaderBar.fillAmount += value;
    }

    public void Open()
    {
        _fillLoaderBar.fillAmount = 0f;

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
    }

    public void Close()
    {
        if (gameObject.activeSelf == true)
            gameObject.SetActive(false);
    }

    //private void StartCloudAnimation()
    //{
    //    Vector3 startPos = _cloudIcon.transform.localPosition;
    //    Vector3 endPos = startPos + _moveOffset;

    //    _moveTween = _cloudIcon.transform.DOMove(endPos, _moveDuration)
    //        .SetEase(_moveEase)
    //        .SetLoops(-1, LoopType.Yoyo)
    //        .OnComplete(() => { });

    //    _fadeTween = _cloudIcon.DOFade(_fadeMin, _fadeDuration)
    //                .SetEase(Ease.InOutSine)
    //                .SetLoops(-1, LoopType.Yoyo);

    //}
}
