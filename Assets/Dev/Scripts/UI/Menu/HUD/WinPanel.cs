using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.Asteroids;

public class WinPanel : Menu, IDisposable
{
    [SerializeField] private Button _continueButton;

    public override void CloseMenu()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public override void OpenMenu()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Dispose()
    {
        _continueButton.onClick.RemoveAllListeners();
    }

    protected override void Initialized()
    {
        base.Initialized();
        
        CloseMenu();

        _continueButton.onClick.AddListener(() => MenuManager.OpenUIMenu());
    }
}
