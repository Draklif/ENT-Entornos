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

    public string GetBindingForAction(string actionName)
    {
        InputAction action = controls.asset.FindAction(actionName);

        if (action == null) return "";

        if (CurrentDevice == null) CurrentDevice = Keyboard.current; // Si no detecta dispositivo, dejamos teclado

        foreach (InputBinding binding in action.bindings)
        {
            if (binding.isComposite || binding.isPartOfComposite) continue;

            if (CurrentDevice is Keyboard && binding.effectivePath.Contains("Keyboard"))
            {
                return binding.ToDisplayString();
            }

            if (CurrentDevice is Gamepad && binding.effectivePath.Contains("Gamepad"))
            {
                return GetGamepadIcon(binding);
            }
        }

        return action.GetBindingDisplayString();
    }

    private GamepadIconMap GetMapForCurrentDevice()
    {
        if (Gamepad.current == null) return null;

        if (Gamepad.current is DualShockGamepad) return gamepadMaps.Find(m => m.gamepadType == GamepadType.PlayStation);

        if (Gamepad.current is XInputController) return gamepadMaps.Find(m => m.gamepadType == GamepadType.Xbox);

        if (Gamepad.current is XInputControllerWindows) return gamepadMaps.Find(m => m.gamepadType == GamepadType.Xbox);

        return gamepadMaps.Find(m => m.gamepadType == GamepadType.Generic);
    }

    private string GetGamepadIcon(InputBinding binding)
    {
        string path = binding.effectivePath;
        GamepadIconMap map = GetMapForCurrentDevice();

        return map != null ? map.GetIcon(path) != "" ? map.GetIcon(path) : map.GetName(path) : map.GetName(path);
    }

}
