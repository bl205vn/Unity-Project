using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f;
    
    public Coroutine Run(string textToType, TMP_Text textLabel)
    {
        // Kiểm tra null trước khi sử dụng
        if (textLabel == null)
        {
            Debug.LogError("TextLabel is null in TypewriterEffect!");
            return null;
        }

        if (string.IsNullOrEmpty(textToType))
        {
            Debug.LogWarning("TextToType is null or empty!");
            textLabel.text = "";
            return null;
        }

        return StartCoroutine(routine: TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        textLabel.text = string.Empty;

        float t = 0f;
        int chatIndex = 0;

        while (chatIndex < textToType.Length)
        {
            t += typewriterSpeed * Time.deltaTime;
            chatIndex = Mathf.FloorToInt(t);
            chatIndex = Mathf.Clamp(value: chatIndex, min: 0, max: textToType.Length);

            textLabel.text = textToType.Substring(startIndex: 0, length: chatIndex);
            yield return null;
        }

        textLabel.text = textToType;
    }
}
