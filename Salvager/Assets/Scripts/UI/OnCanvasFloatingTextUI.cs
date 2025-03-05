using System.Collections;
using TMPro;
using UnityEngine;

public interface IFloatingTextService
{
    void Show(string message, Vector3 worldPosition, Color textColor, float duration);
}

public class FloatingTextService : MonoBehaviour, IFloatingTextService
{
    [Tooltip("The prefab used for displaying floating text.")] [SerializeField]
    private TextMeshProUGUI floatingTextPrefab;

    [Tooltip("The canvas to which the floating text will be added.")] [SerializeField]
    private Canvas canvas;

    [Tooltip("The speed at which the text floats upwards.")] [SerializeField]
    private float floatSpeed = 3f;

    [Tooltip("The duration for which the text is visible.")] [SerializeField]
    private float defaultDuration = 1.5f;

    /// <summary>
    ///   Displays floating text on the screen.
    /// </summary>
    /// <param name="message">The text to display.</param>
    /// <param name="worldPosition">The world position where the text should appear.</param>
    /// <param name="textColor">The color of the text.</param>
    /// <param name="duration">The duration for which the text is visible.</param>
    public void Show(
        string message,
        Vector3 worldPosition,
        Color textColor,
        float duration = 1.5f)
    {
        if (floatingTextPrefab == null || canvas == null)
        {
            Debug.LogError(
                "FloatingTextPrefab or Canvas is not assigned! Floating text will not" +
                " be displayed.");
            return;
        }

        // Convert world position to screen position
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(
            Camera.main,
            worldPosition);

        // Instantiate the floating text object
        TextMeshProUGUI floatingText = Instantiate(
            floatingTextPrefab,
            canvas.transform);
        floatingText.transform.position = screenPosition;
        floatingText.text = message;
        floatingText.color = textColor;

        // Start floating and fading coroutine
        StartCoroutine(FloatingTextEffect(floatingText, duration));
    }

    /// <summary>
    ///   Coroutine that handles the floating and fading effect of the text.
    /// </summary>
    /// <param name="textObject">The TextMeshProUGUI object to animate.</param>
    /// <param name="duration">The duration of the animation.</param>
    private IEnumerator FloatingTextEffect(
        TextMeshProUGUI textObject,
        float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = textObject.transform.position;
        Vector3 endPos = startPos + new Vector3(0, floatSpeed, 0);
        Color startColor = textObject.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            textObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            textObject.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                1 - t);
            yield return null;
        }

        Destroy(textObject.gameObject);
    }
}