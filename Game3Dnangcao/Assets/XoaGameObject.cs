using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XoaGameObject : MonoBehaviour
{
    [SerializeField] private float delay = 5.5f;
    void Start()
    {
        StartCoroutine(DestroyGameobject());
    }

    IEnumerator DestroyGameobject()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
