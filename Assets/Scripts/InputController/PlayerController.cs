using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MainMenuManager mainMenuManager;

    private InputSystem_Actions _inputSystemActions;
    private Action<Ant, Vector3> _onSelectTarget;
    private Ant _ant;

    private async void Awake()
    {
        mainMenuManager = await MainMenuManager.GetInstanceAsync();
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.Target.performed += OnTarget;
        _inputSystemActions.Player.ToggleMainMenu.performed += OnToggleMainMenu;
    }

    public void Initialize(Ant ant, Action<Ant, Vector3> onSelectTarget)
    {
        _ant = ant;
        _onSelectTarget = onSelectTarget;
    }

    private void OnEnable() => _inputSystemActions.Enable();
    private void OnDisable() => _inputSystemActions.Disable();


    private void OnTarget(InputAction.CallbackContext context)
    {
        if (_ant.GameSettings.isPaused || !_ant.GameSettings.isGameStarted)
            return;

        if (!_ant.GameSettings.isPlayerControlled) 
            return;

        // Player Controlled: Get mouse position
        Vector2 screenPosition = _inputSystemActions.Player.PointerPosition.ReadValue<Vector2>();
        _onSelectTarget?.Invoke(_ant, screenPosition);
    }

    private void OnToggleMainMenu(InputAction.CallbackContext context)
    {
        mainMenuManager.ShowMainMenu();
    }
}
