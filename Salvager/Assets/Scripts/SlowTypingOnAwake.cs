using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SlowTypingOnAwake : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;

    [SerializeField] [TextArea] private string fullText;
    private Coroutine _typingCoroutine;

    private void OnEnable()
    {
        StartTyping();
    }

    public void StartTyping()
    {
        textComponent.text = "";
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        textComponent.text = "";
        string processedText = fullText;
        Regex pauseRegex = new Regex(@"\[PAUSE=(\d+(\.\d*)?)\]");

        int lastIndex = 0;
        foreach (Match match in pauseRegex.Matches(fullText))
        {
            string textBeforePause = fullText.Substring(lastIndex, match.Index - lastIndex);
            yield return TypeCharacters(textBeforePause);

            float pauseTime = float.Parse(match.Groups[1].Value);
            yield return new WaitForSeconds(pauseTime);

            lastIndex = match.Index + match.Length;
        }

        // Type remaining text after last pause
        yield return TypeCharacters(fullText.Substring(lastIndex));
    }

    private IEnumerator TypeCharacters(string text)
    {
        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}