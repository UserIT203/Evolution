using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShopMenu : Menu
{
    [Inject] private ChestManager _chestManager;

    [SerializeField] private RawImage _chestOpenView;

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

    private void OpenChest(Rarity chestRarity)
    {
        _chestOpenView.gameObject.SetActive(true);
        _chestManager.OpenChest(chestRarity);
    }

    protected override void Initialized()
    {
        
    }
}
