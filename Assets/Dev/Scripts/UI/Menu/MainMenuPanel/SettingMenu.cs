using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : Menu
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private bool _isHUD = false;

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
