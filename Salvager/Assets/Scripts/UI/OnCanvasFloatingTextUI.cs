using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IOnCanvasFloatingTextUI
{
    void Show(string message, Vector3 worldPosition, Color textColor, float duration = 1.5f);
}

public class OnCanvasFloatingTextUI : MonoBehaviour, IOnCanvasFloatingTextUI
{
    public TextMeshProUGUI floatingTextPrefab; // Assign this in the inspector
    public Canvas canvas;
    
    public void Show(string message, Vector3 screenPosition, Color textColor, float duration = 1.5f)
    {
        if (floatingTextPrefab == null || canvas == null)
        {
            Debug.LogError("FloatingTextPrefab or Canvas is not assigned!");
            return;
        }
        
        // Instantiate the floating text object
        var floatingText = Instantiate(floatingTextPrefab, canvas.transform);
        floatingText.transform.position = screenPosition;
        floatingText.text = message;
        floatingText.color = textColor;
        
        // Start floating and fading coroutine
        StartCoroutine(FloatingTextEffect(floatingText, duration));
    }
    
    private System.Collections.IEnumerator FloatingTextEffect(TextMeshProUGUI textObject, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = textObject.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 3, 0);
        Color startColor = textObject.color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            textObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            textObject.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);
            yield return null;
        }
        
        Destroy(textObject.gameObject);
    }
}