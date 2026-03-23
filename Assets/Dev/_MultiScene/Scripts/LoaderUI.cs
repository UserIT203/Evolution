using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LoaderUI : MonoBehaviour
{
    [SerializeField] private Image _loadingBar;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        CloseLoaderUI();
    }


    public void OpenLoaderUI(float progress)
    {
        _loadingBar.fillAmount = progress;
        
        if(_canvasGroup.alpha != 1)
            _canvasGroup.DOFade(1f, 1f).SetEase(Ease.InOutSine);
    }

    public void CloseLoaderUI()
    {
        _canvasGroup.DOFade(0f, 2f).SetEase(Ease.Linear);
    }
}
