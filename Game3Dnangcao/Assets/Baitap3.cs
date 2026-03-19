using System.Collections;
using UnityEngine;

public class Baitap3 : MonoBehaviour
{
    [SerializeField] private GameObject[] capsules;
    [SerializeField] private Vector3[] targetPositions;
    
    void Start()
    {
        if (capsules == null || capsules.Length == 0)
        {
            capsules = new GameObject[]
            {
                GameObject.Find("Capsule"),
                GameObject.Find("Capsule (1)"),
                GameObject.Find("Capsule (2)")
            };
        }
        
        if (targetPositions == null || targetPositions.Length == 0)
        {
            targetPositions = new Vector3[]
            {
                new Vector3(5f, 0f, 0f),
                new Vector3(0f, 0f, 5f),
                new Vector3(-5f, 0f, 0f)
            };
        }
        
        StartCoroutine(MoveSequentially());
    }

    IEnumerator MoveSequentially()
    {
        for (int i = 0; i < capsules.Length && i < targetPositions.Length; i++)
        {
            if (capsules[i] != null)
            {
                capsules[i].transform.position = targetPositions[i];
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
