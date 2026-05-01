using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class LosePanel : Menu
{
    [Inject] private YandexSDK _yandexSDK;
    [Inject] private LevelUpgrade _levelUpgrade;

    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _doubleCoinButton;
    [SerializeField] private TMP_Text _earnedCoinsText;

    private void OnDisable()
    {
        _continueButton.onClick.RemoveAllListeners();
        _doubleCoinButton.onClick.RemoveAllListeners();
    }

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();

        _earnedCoinsText.text = _levelUpgrade.LevelEarnedCoins.ToString();
    }

    public override void Initialized()
    {
        base.Initialized();
        
        CloseMenu();

        _continueButton.onClick.AddListener(() => MenuManager.OpenUIMenu());
        _doubleCoinButton.onClick.AddListener(RewardAction);
    }

    private void RewardAction()
    {
        _yandexSDK.ShowRewardADV("doubleCoint",
            () => 
            {
                _levelUpgrade.AddCoin(_levelUpgrade.LevelEarnedCoins);
                MenuManager.OpenUIMenu();
            });  
    }
}
