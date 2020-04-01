using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;
    GameObject Potion;
    public float SpawnDelay;
    int MaxSpawn = 1;
    int CurSpawnCount = 0;

    IEnumerator coroutine;


    void Start()
    {
        coroutine = SpawnPrefab();
        StartCoroutine(coroutine);
    }

    IEnumerator SpawnPrefab()
    {
        while(true)
        {
            yield return new WaitForSeconds(SpawnDelay);
            if(CurSpawnCount < MaxSpawn)
            {                
                // Spawn prefab at this position
                if(Potion == null)
                {
                    Potion = Instantiate(PrefabToSpawn, transform.position, Quaternion.identity);

                    // Child to spawner
                    Potion.transform.parent = transform;

                    // Increase spawn count
                    CurSpawnCount++;
                }
                else if (!Potion.activeInHierarchy)
                {
                    // Reactivate potion
                    Potion.SetActive(true);
                    // Increase spawn count
                    CurSpawnCount++;
                }               
            }
        }        
    }

    internal void HidePotion()
    {
        if(Potion != null)
        {
            Potion.SetActive(false);

            // Decrease spawn count
            CurSpawnCount--;
        }
    }
}
