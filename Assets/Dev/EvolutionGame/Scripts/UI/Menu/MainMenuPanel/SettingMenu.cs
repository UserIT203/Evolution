using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingMenu : Menu
{
    [Inject] private LocalizationSelector _localizationSelector;

    [System.Serializable]
    private struct LanguageButton
    {
        public Button Button;
        public int LanguageIndex;
    }

    [SerializeField] private Button _closeButton;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private bool _isHUD = false;

    [Header("<color=green>Language Button</color>")]
    [SerializeField] private LanguageButton[] _languageButton;

    private void OnEnable()
    {
        _musicSlider.onValueChanged.AddListener
            (
                (float v) => AudioManager.Instance.SetVolume(v, "musicVolume")
            );
        _sfxSlider.onValueChanged.AddListener
            (
                (float v) => AudioManager.Instance.SetVolume(v, "sfxVolume")
            );

        if (_isHUD == false)
            _closeButton.onClick.AddListener(() => MenuManager.OpenMenu(0));
        else
            _closeButton.onClick.AddListener(CloseMenu);
    }

    private void OnDisable()
    {
        _musicSlider.onValueChanged.RemoveAllListeners();
        _sfxSlider.onValueChanged.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        if (_languageButton.Length <= 0) return;

        foreach (var item in _languageButton)
            item.Button.onClick.RemoveAllListeners();
    }

    public override void Initialized()
    {
        if (_languageButton.Length <= 0) return;

        foreach (var item in _languageButton)
        {
            item.Button.onClick.AddListener(() =>
            {
                _localizationSelector.SetLocalization(item.LanguageIndex).Forget();
            });
        }
    }

    public override void CloseMenu()
    {
        _canvasGroup.Hide();
    }

    public override void OpenMenu()
    {
        _canvasGroup.Show();

        _musicSlider.value = AudioManager.Instance.GetVolumeValue("musicVolume");
        _sfxSlider.value = AudioManager.Instance.GetVolumeValue("sfxVolume");
    }
}
