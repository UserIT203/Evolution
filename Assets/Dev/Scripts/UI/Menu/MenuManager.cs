using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class MenuManager : MonoBehaviour
{
    [Header("HUD Elements Links")]
    [SerializeField] private TMP_Text _levelLabel;
    [SerializeField] private TMP_Text _coinValueHUDText;
    [SerializeField] private TMP_Text _gemValueHUDText;
    [SerializeField] private Button _pauseButton;

    [Header("UI Links")]
    [SerializeField] private TMP_Text _headerText;
    [SerializeField] private TMP_Text _coinValueUIText;
    [SerializeField] private TMP_Text _gemValueUIText;

    [Header("Main Options")]
    [SerializeField] private Menu[] _menus;
    [SerializeField] private int _startMenuIndex;
    [SerializeField] private List<Menu> _hudMenu;

    [Header("Menu Open Buttons")]
    [SerializeField] private List<MenuOpenButtons> _buttons = new();

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
    
        _levelManager = levelManager;
    }

    private void OnValidate()
    {
        if(_menus == null || _menus.Length == 0 || _menus.Length == _buttons.Count) return;

        for(int i = _buttons.Count; i < _menus.Length; i++)
        {
            MenuOpenButtons button = new();
            button.MenuIndex = i;

            _buttons.Add(button);
        }
    }

    private void OnEnable()
    {
        _pauseButton.onClick.AddListener(() => GetHUDPanel<PausePanel>().OpenMenu());

        if (_buttons == null || _buttons.Count == 0) return;

        foreach (var button in _buttons)
        {
            button.Button.onClick.AddListener(() => OpenMenu(button.MenuIndex));
        }
    }

    private void OnDisable()
    {
        _pauseButton.onClick.RemoveAllListeners();

        if (_buttons == null || _buttons.Count == 0) return;

        foreach (var button in _buttons)
        {
            button.Button.onClick.RemoveAllListeners();
        }
    }

    private void Start()
    {
        _cavasGroup = GetComponent<CanvasGroup>();

        CloseAllPanel();
        OpenMenu(_startMenuIndex);
        OpenUIMenu();

        foreach (var menu in _menus)
            menu.MenuManager = this;

        foreach (var hudPanel in _hudMenu)
            hudPanel.MenuManager = this;
    }

    public void OpenMenu(int menuIndex)
    {
        _currentOpenMenu?.CloseMenu();

        _currentOpenMenu = _menus[menuIndex];
        _currentOpenMenu.OpenMenu();

        _headerText.text = _buttons.Find(b => b.MenuIndex == menuIndex).MenuName;
    }

    public void CloseUIMenu()
    {
        _cavasGroup.alpha = 0f;
        _cavasGroup.blocksRaycasts = false;
        _cavasGroup.interactable = false;

        _levelLabel.text = string.Format(_levelLabel.text, _levelManager.CurrentSelectedLevel + 1);

        CloseAllPanel();
    }

    public void OpenUIMenu()
    {
        _cavasGroup.alpha = 1f;
        _cavasGroup.blocksRaycasts = true;
        _cavasGroup.interactable = true;

        foreach (var hudPanel in _hudMenu)
            hudPanel.CloseMenu();

        OpenMenu(_startMenuIndex);
    }

    public T GetHUDPanel<T>() where T : Menu
    {
        return _hudMenu.OfType<T>().FirstOrDefault();
    }

    private void CloseAllPanel()
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

    
}

[System.Serializable]
public struct MenuOpenButtons
{
    public string MenuName;
    public int MenuIndex;
    public Button Button;
}
