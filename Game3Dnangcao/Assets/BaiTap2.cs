using System.Collections;
using UnityEngine;

public class BaiTap2 : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float fadeDuration = 5f;
    
    private bool isFading = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && targetMaterial != null && !isFading)
        {
            StartCoroutine(FadeOut());
        }
    }
    
    IEnumerator FadeOut()
    {
        isFading = true;
        float elapsedTime = 0f;
        Color originalColor = targetMaterial.color;
        float startAlpha = originalColor.a;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            targetMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        targetMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        isFading = false;
    }
}
