using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<GamepadIconMap> gamepadMaps;

    public static InputManager Instance { get; private set; }
    public InputDevice CurrentDevice { get; private set; }
    public Gamepad CurrentGamepad { get; private set; }

    private PlayerController controls;

    // Eventos de Player
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnCameraMove;
    public event Action<float> OnCameraZoom;
    public event Action OnRecenterCamera;
    public event Action OnInteract;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new PlayerController();

        // Values
        controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        controls.Player.CameraMove.performed += ctx => OnCameraMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.CameraMove.canceled += ctx => OnCameraMove?.Invoke(Vector2.zero);
        controls.Player.Zoom.performed += ctx => OnCameraZoom?.Invoke(ctx.ReadValue<float>());
        controls.Player.Zoom.canceled += ctx => OnCameraZoom?.Invoke(0);

        // Buttons
        controls.Player.RecenterCamera.performed += _ => OnRecenterCamera?.Invoke();
        controls.Player.Interact.performed += _ => OnInteract?.Invoke();

        // Detect Change
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && obj is InputAction action)
        {
            InputDevice device = action.activeControl.device;

            // Ignora si la última acción fue de Mouse
            if (device is Mouse) return;

            // Guardamos el dispositivo del último control usado
            CurrentDevice = device;

            // Guardamos el gamepad activo
            CurrentGamepad = Gamepad.current;
        }
    }

    void OnDestroy()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public string GetKeyName(string actionName)
    {
        InputBinding? binding = FindBindingForAction(actionName);
        if (binding == null) return "";
        return GetBindingName(binding.Value);
    }

    public Sprite GetKeyIcon(string actionName)
    {
        InputBinding? binding = FindBindingForAction(actionName);
        if (binding == null) return null;
        return GetBindingIcon(binding.Value);
    }

    InputBinding? FindBindingForAction(string actionName)
    {
        var action = controls.asset.FindAction(actionName);
        if (action == null) return null;

        CurrentDevice ??= Keyboard.current;

        foreach (var binding in action.bindings)
        {
            if (binding.isComposite || binding.isPartOfComposite) continue;

            if (BindingMatchesDevice(binding)) return binding;
        }

        return null;
    }

    bool BindingMatchesDevice(InputBinding binding)
    {
        string path = binding.effectivePath;

        if (CurrentDevice is Keyboard) return path.Contains("Keyboard");

        if (CurrentDevice is Gamepad) return path.Contains("Gamepad");

        return false;
    }

    private GamepadIconMap GetMapForCurrentDevice()
    {
        if (CurrentDevice is Keyboard) return gamepadMaps.Find(m => m.gamepadType == GamepadType.PC);

        if (CurrentGamepad is DualShockGamepad) return gamepadMaps.Find(m => m.gamepadType == GamepadType.PlayStation);

        if (CurrentGamepad is XInputController or XInputControllerWindows) return gamepadMaps.Find(m => m.gamepadType == GamepadType.Xbox);

        return gamepadMaps.Find(m => m.gamepadType == GamepadType.Generic);
    }

    string GetBindingName(InputBinding binding)
    {
        GamepadIconMap map = GetMapForCurrentDevice();
        return map?.GetName(binding.effectivePath);
    }

    Sprite GetBindingIcon(InputBinding binding)
    {
        GamepadIconMap map = GetMapForCurrentDevice();
        return map?.GetIcon(binding.effectivePath);
    }

}
