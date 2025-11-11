using TMPro;
using UnityEngine;

public class InteractionUIManager : MonoBehaviour
{
    public static InteractionUIManager Instance { get; private set; }

    [Header("Referencias")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI interactText;

    [Header("Animación")]
    public float fadeSpeed = 5f;

    private bool isVisible = false;
    private string currentText = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        float targetAlpha = isVisible ? 1 : 0;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    public void Show(string text)
    {
        if (text == currentText && isVisible) return;

        currentText = text;
        interactText.text = text;
        isVisible = true;
    }

    public void Hide()
    {
        isVisible = false;
    }
}
