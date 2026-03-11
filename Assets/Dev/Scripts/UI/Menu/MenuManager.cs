using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [Header("HUD Elements Links")]
    [SerializeField] private TMP_Text _coinValueHUDText;
    [SerializeField] private TMP_Text _gemValueHUDText;

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

    private Menu _currentOpenMenu;

    [Inject]
    public void Constract(
        GameManager gameManager, 
        GlobalManager globalManager, 
        LevelUpgrade levelUpgrade)
    {
        gameManager.onPlay += CloseAllPanel;

        globalManager.onChangeCoin += ChangeGemText;
        levelUpgrade.onChangeMoney += ChangeCointText;
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
        if (_buttons == null || _buttons.Count == 0) return;

        foreach (var button in _buttons)
        {
            button.Button.onClick.AddListener(() => OpenMenu(button.MenuIndex));
        }
    }

    private void OnDisable()
    {
        if (_buttons == null || _buttons.Count == 0) return;

        foreach (var button in _buttons)
        {
            button.Button.onClick.RemoveAllListeners();
        }
    }

    private void Start()
    {
        CloseAllPanel();
        OpenMenu(_startMenuIndex);

        foreach (var menu in _menus)
            menu.MenuManager = this;
    }

    public void OpenMenu(int menuIndex)
    {
        _currentOpenMenu?.CloseMenu();

        _currentOpenMenu = _menus[menuIndex];
        _currentOpenMenu.OpenMenu();

        _headerText.text = _buttons.Find(b => b.MenuIndex == menuIndex).MenuName;
    }

    private void CloseAllPanel()
    {
        foreach (var menu in _menus)
        {
            if(menu.IsClosed == false) 
                menu.CloseMenu();
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

    public T GetHUDPanel<T>() where T : Menu
    {
        return _hudMenu.OfType<T>().FirstOrDefault();
    }
}

[System.Serializable]
public struct MenuOpenButtons
{
    public string MenuName;
    public int MenuIndex;
    public Button Button;
}
