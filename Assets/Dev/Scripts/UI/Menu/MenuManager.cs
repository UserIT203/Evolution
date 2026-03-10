using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [Header("UI Links")]
    [SerializeField] private TMP_Text _coinCount;
    [SerializeField] private TMP_Text _gemCount;

    [Header("Main Options")]
    [SerializeField] private Menu[] _menus;
    [SerializeField] private int _startMenuIndex;

    [Header("Menu Open Buttons")]
    [SerializeField] private List<MenuOpenButtons> _buttons = new();

    private Menu _currentOpenMenu;

    [Inject]
    public void Constract(GameManager gameManager, GlobalManager globalManager, LevelUpgrade levelUpgrade)
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
    }

    private void OpenMenu(int menuIndex)
    {
        _currentOpenMenu?.CloseMenu();

        _currentOpenMenu = _menus[menuIndex];
        _currentOpenMenu.OpenMenu();
    }

    private void CloseAllPanel()
    {
        foreach (var menu in _menus)
        {
            menu.CloseMenu();
        }
    }

    private void ChangeGemText(int value) => _gemCount.text = value.ToString();
    private void ChangeCointText(int value) => _coinCount.text = value.ToString();
}

[System.Serializable]
public struct MenuOpenButtons
{
    public int MenuIndex;
    public Button Button;
}
