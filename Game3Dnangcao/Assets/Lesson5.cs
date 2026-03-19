using UnityEngine;
using System.Collections;

public class Lesson5 : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ScaleObject(transform, new Vector3(5f, 5f, 5f), 1f));
    }

    IEnumerator ScaleObject(Transform obj, Vector3 targetScale, float duration)
    {
        Vector3 startScale = obj.localScale;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            obj.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.localScale = targetScale;
    }
}
