using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRange = 1.5f;
    public LayerMask interactableLayer;

    private IInteractable currentTarget;

    void Start()
    {
        // Suscribirse a evento del InputManager
        InputManager.Instance.OnInteract += HandleInteract;
    }

    void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);

        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentTarget = interactable;
                string key = GetInteractKey();
                string uiText = $"[{key}] {interactable.GetInteractText()}";
                InteractionUIManager.Instance.Show(uiText);
                return;
            }
        }

        currentTarget = null;
        InteractionUIManager.Instance.Hide();
    }

    void HandleInteract()
    {
        currentTarget?.Interact(gameObject);
    }

    string GetInteractKey()
    {
        string binding = InputManager.Instance.GetBindingForAction("Player/Interact");
        return binding ?? "E";
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
