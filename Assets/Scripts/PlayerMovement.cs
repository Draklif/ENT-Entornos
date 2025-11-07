using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    private Vector2 playerMoveInput;

    void Start()
    {
        // Suscribirse a evento del InputManager
        InputManager.Instance.OnMove += HandleMove;
    }

    void HandleMove(Vector2 input)
    {
        playerMoveInput = input;
    }

    void Update()
    {
        // Movimiento top down
        Vector3 movement = new Vector3(playerMoveInput.x, playerMoveInput.y, 0);

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
