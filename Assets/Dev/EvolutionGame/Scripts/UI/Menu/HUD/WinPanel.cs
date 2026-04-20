using System;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : Menu, IDisposable
{
    [SerializeField] private Button _continueButton;

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();
    }

    public void Dispose()
    {
        _continueButton.onClick.RemoveAllListeners();
    }

    public override void Initialized()
    {
        base.Initialized();
        
        CloseMenu();

        _continueButton.onClick.AddListener(() => MenuManager.OpenUIMenu());
    }
}
