using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

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
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }
}
