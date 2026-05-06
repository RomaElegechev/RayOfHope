using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputSystem _inputSystem;

    public event Action OnPauseStarted;

    private void Awake()
    {
        Instance = this;

        _inputSystem = new InputSystem();
        _inputSystem.Enable();

        _inputSystem.Game.Pause.started += Pause_started;
    }

    private void Pause_started(InputAction.CallbackContext obj) => OnPauseStarted?.Invoke();
}
