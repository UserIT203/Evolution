using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopInput
{
    private PlayerAction _playerInput;

    public Action<int> onGetPressButtonIndex;
    public Action onPressButtonAbility;
    public Action onPressPause;

    public DesktopInput()
    {
        Debug.Log("<color=red>Input Init</color>");

        _playerInput = new PlayerAction();

        InstallAction();
    }

    public void InstallAction()
    {
        _playerInput.Player.SpawnUnit_1.performed += ctx => PressButton(1); 
        _playerInput.Player.SpawnUnit_2.performed += ctx => PressButton(2); 
        _playerInput.Player.SpawnUnit_3.performed += ctx => PressButton(3);

        _playerInput.Player.UseAbility.performed += ctx => onPressButtonAbility?.Invoke();

        _playerInput.Player.Pause.performed += ctx => onPressPause?.Invoke();
    }

    public void EnableInput() => _playerInput.Enable();
    public void DisableInput() => _playerInput.Disable();

    public void PressButton(int index)
    {
        Debug.Log($"<color=green>Input index</color> {index}");

        onGetPressButtonIndex?.Invoke(index);
    }
}
