using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;
    public float SpawnDelay;
    public int MaxSpawn;
    int CurSpawnCount = 0;

    IEnumerator coroutine;


    void Start()
    {
        coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
    }

    IEnumerator SpawnPrefab()
    {
        while (CurSpawnCount < MaxSpawn)
        {
            yield return new WaitForSeconds(SpawnDelay);
            // Spawn prefab at this position
            Instantiate(PrefabToSpawn, transform.position, Quaternion.identity);

            // Increase spawn count
            CurSpawnCount++;
        }
    }
}
