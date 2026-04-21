using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SceneLoaderView : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] private Image _loadIcon;
    [SerializeField] private Image _cloudIcon;

    private Tween _loadIconTween;
    private SceneLoader _loader;

    [Inject] 
    public void Construct(SceneLoader loader)
    {
        _loader = loader;

        _loader.onStartLoadScene += Open;
        _loader.onEndLoadScene += Close;
    }

    private void Awake()
    {
        Open();
        DontDestroyOnLoad(gameObject);

        _loadIconTween = _loadIcon.rectTransform.DOLocalRotate(
            new Vector3(0, 360, 0),
            0.5f,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Incremental)
        .SetAutoKill(false);
    }

    private void OnDestroy()
    {
        _loadIconTween?.Kill();
        _loader.onStartLoadScene -= Open;
        _loader.onEndLoadScene -= Close;
    }

    public void Open()
    {
        _loadIconTween?.Play();

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
    }

    public void Close()
    {
        _loadIconTween?.Pause();

        if (gameObject.activeSelf == true)
            gameObject.SetActive(false);
    }
}
