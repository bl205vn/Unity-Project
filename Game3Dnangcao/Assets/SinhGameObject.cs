using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameObject : MonoBehaviour
{
    [SerializeField] private GameObject test;
    [SerializeField] private float spawnAreaX = 10f;
    [SerializeField] private float spawnAreaZ = 10f;
    [SerializeField] private int maxSpawnCount = 10;
    
    void Start()
    {
        StartCoroutine(WaitAndSpawnEnemy());
    }

    IEnumerator WaitAndSpawnEnemy()
    {
        int spawnCount = 0;
        while (spawnCount < maxSpawnCount)
        {
            yield return new WaitForSeconds(2f);
            float randomX = Random.Range(-spawnAreaX / 2f, spawnAreaX / 2f);
            float randomZ = Random.Range(-spawnAreaZ / 2f, spawnAreaZ / 2f);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);
            Instantiate(test, randomPosition, Quaternion.identity);
            spawnCount++;
        }
    }
}