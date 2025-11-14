using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    private Vector2 playerMoveInput;
    private Rigidbody2D rb;

    void Start() 
    {
        // Configuración del Rigidbody2D para el movimiento
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Suscribirse a evento del InputManager
        InputManager.Instance.OnMove += HandleMove;
    }

    void HandleMove(Vector2 input)
    {
        playerMoveInput = input;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = playerMoveInput * moveSpeed;
    }
}
