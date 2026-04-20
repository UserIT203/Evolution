using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LosePanel : Menu
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _rebornButton;

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();
    }

    public override void Initialized()
    {
        base.Initialized();
        
        CloseMenu();

        _continueButton.onClick.AddListener(() => MenuManager.OpenUIMenu());
    }
}
