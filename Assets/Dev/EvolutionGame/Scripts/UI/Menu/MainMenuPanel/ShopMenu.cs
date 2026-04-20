using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopMenu : Menu
{
    [Header("Shop Popup UI Links")]
    [SerializeField] private CanvasGroup _shopPopupCanvas;
    [SerializeField] private Image _shopPopupBackground;
    [SerializeField] private TMP_Text _shopPopupText;

    private Sequence _shopPopupSequence;

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
        _shopPopupCanvas.Hide();

        _shopPopupSequence?.Kill();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();
    }

    public override void Initialized()
    {
        _shopPopupCanvas.Hide();
    }

    public void OpenShopPopup()
    {
        _shopPopupSequence = DOTween.Sequence();
        _shopPopupText.enabled = false;
        _shopPopupCanvas.Show();

        _shopPopupSequence
            .Append(
                _shopPopupBackground.transform.DOScale(1f, 0.25f).From(0f)
                .OnComplete(() => _shopPopupText.enabled = true)
                )
            .AppendInterval(0.5f)
            .Append(_shopPopupBackground.transform.DOScale(0f, 0.25f))
            .OnComplete(() => _shopPopupCanvas.Hide());
    }
}
