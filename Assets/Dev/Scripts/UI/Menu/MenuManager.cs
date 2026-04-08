using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

[RequireComponent(typeof(CanvasGroup))]
public class MenuManager : MonoBehaviour, IInitialized
{
    private const string LOCALIZATION_TABLE_UI = "MenuLabels"; 

    [Inject] private DesktopInput _desktopInput;

    [Header("HUD Elements Links")]
    [SerializeField] private LocalizeStringEvent _levelLabelStringEvent;
    [SerializeField] private TMP_Text _coinValueHUDText;
    [SerializeField] private TMP_Text _gemValueHUDText;
    [SerializeField] private Button _pauseButton;

    [Header("UI Links")]
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _previousLevelButton;
    [SerializeField] private LocalizeStringEvent _headerTextStringEvent;
    [SerializeField] private TMP_Text _coinValueUIText;
    [SerializeField] private TMP_Text _gemValueUIText;

    [Header("Main Options")]
    [SerializeField] private Menu[] _menus;
    [SerializeField] private int _startMenuIndex;
    [SerializeField] private List<Menu> _hudMenu;

    [Header("Menu Open Buttons")]
    [SerializeField] private List<MenuOpenButtons> _menuSetting = new();

    private CanvasGroup _cavasGroup;
    private Menu _currentOpenMenu;

    private LevelManager _levelManager;

    [Inject]
    public void Constract(
        GameManager gameManager, 
        GlobalManager globalManager, 
        LevelUpgrade levelUpgrade,
        LevelManager levelManager)
    {
        gameManager.onPlay += CloseAllPanel;
        gameManager.onPlay += CloseUIMenu;

        gameManager.onLoseLevel += () => GetHUDPanel<LosePanel>().OpenMenu();

        levelManager.onEraCompleted += () => GetHUDPanel<WinEraPanel>().OpenMenu();
        levelManager.onLevelCompleted += () => GetHUDPanel<WinPanel>().OpenMenu();

        globalManager.onChangeCoin += ChangeGemText;
        levelUpgrade.onChangeMoney += ChangeCointText;

        _nextLevelButton.onClick.AddListener(() => levelManager.SetNextLevel());
        _previousLevelButton.onClick.AddListener(() => levelManager.SetPreviousLevel());
    
        _levelManager = levelManager;
    }

    private void OnEnable()
    {
        _pauseButton.onClick.AddListener(OpenPauseMenu);

        if (_menuSetting == null || _menuSetting.Count == 0) return;

        foreach (var button in _menuSetting)
        {
            button.Button.onClick.AddListener(() => OpenMenu(button.MenuIndex));
        }
    }

    private void OnDisable()
    {
        _pauseButton.onClick.RemoveAllListeners();
        _nextLevelButton.onClick.RemoveAllListeners();
        _previousLevelButton.onClick.RemoveAllListeners();

        if (_menuSetting == null || _menuSetting.Count == 0) return;

        foreach (var button in _menuSetting)
        {
            button.Button.onClick.RemoveAllListeners();
        }
    }

    public void Initialized()
    {
        if (_desktopInput != null) _desktopInput.onPressPause += OpenPauseMenu;

        _cavasGroup = GetComponent<CanvasGroup>();

        CloseAllPanel();
        OpenMenu(_startMenuIndex);
        OpenUIMenu();

        foreach (var menu in _menus)
        {
            menu.MenuManager = this;
            menu.Initialized();
        }
            
        foreach (var hudPanel in _hudMenu)
        {
            hudPanel.MenuManager = this;
            hudPanel.Initialized();
        }    
    }

    public void OpenMenu(int menuIndex)
    {
        _currentOpenMenu?.CloseMenu();
        _currentOpenMenu = _menus[menuIndex];
        _currentOpenMenu.OpenMenu();

        MenuOpenButtons menu = _menuSetting.Find(b => b.MenuIndex == menuIndex);

        _headerTextStringEvent.StringReference.SetReference(LOCALIZATION_TABLE_UI, menu.Entry);
        _headerTextStringEvent.RefreshString();
    }

    public T GetUIMenu<T>() where T : Menu
    {
        return _menus.OfType<T>().First();
    }

    public void CloseUIMenu()
    {
        _cavasGroup.Hide();

        if (_levelLabelStringEvent.StringReference.Arguments == null)
            _levelLabelStringEvent.StringReference.Arguments = new object[1];

        _levelLabelStringEvent.StringReference.Arguments[0] = _levelManager.CurrentSelectedLevel + 1;
        _levelLabelStringEvent.RefreshString();
        
        CloseAllPanel();
    }

    public void OpenUIMenu()
    {
        _cavasGroup.Show();

        foreach (var hudPanel in _hudMenu)
            hudPanel.CloseMenu();

        OpenMenu(_startMenuIndex);
    }

    public T GetHUDPanel<T>() where T : Menu
    {
        return _hudMenu.OfType<T>().FirstOrDefault();
    }

    public void CloseAllPanel()
    {
        foreach (var menu in _menus)
        {
            if (menu.IsClosed == false) menu.CloseMenu();
        }
    }

    private void ChangeGemText(int value)
    {
        _gemValueHUDText.text = value.ToString();
        _gemValueUIText.text = value.ToString();
    }

    private void ChangeCointText(int value)
    {
        _coinValueHUDText.text = value.ToString();
        _coinValueUIText.text = value.ToString();
    }

    private void OpenPauseMenu() => GetHUDPanel<PausePanel>().OpenMenu();
}

[System.Serializable]
public struct MenuOpenButtons
{
    public TableEntryReference Entry;
    public int MenuIndex;
    public Button Button;
}
