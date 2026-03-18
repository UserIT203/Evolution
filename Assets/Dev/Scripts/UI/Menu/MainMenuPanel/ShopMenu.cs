using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShopMenu : Menu
{
    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }
}
