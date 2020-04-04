using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    // Spawn points
    public Transform SpawnPoint01;
    public Transform SpawnPoint02;
    Transform CurSpawnPoint;

    // Units
    public GameObject BasicUnitPrefab;
    public GameObject HeavyUnitPrefab;
    public GameObject CommanderPrefab;    

    // Gold
    public int GoldPerWave = 150;
    public int GoldIncreasePerWave = 50;
    int CurGold = 0;

    // Costs
    public int BasicUnitSpawnCost = 50;
    public int HeavyUnitSpawnCost = 100;
    public int CommanderSpawnCost = 350;

    // Delays
    public float WaveDelayMax; // Delay between waves
    float CurWaveDelay;
    float WaveTime;
    public float SpawnDelay; // Delay between spawns

    // Limits
    public int MinBasicUnits = 4;
    int CurBasicUnits = 0;
    public int MinHeavyUnits = 2;
    int CurHeavyUnits = 0;
    int MaxCommanders = 1;
    int CurCommanders = 0;

    // Texts
    public Text CurrentWaveText;
    public Text CountdownText;

    // Wave
    int WaveCount = 0;

    bool CommanderSpawned = false;

    IEnumerator coroutine;
    IEnumerator coroutine02;


    void Start()
    {
        // Init spawn point
        CurSpawnPoint = SpawnPoint01;

        // Init wave time
        WaveTime = WaveDelayMax;

        // Init wave delays
        CurWaveDelay = WaveDelayMax;

        // Init texts
        CurrentWaveText.text = WaveCount.ToString();
        CountdownText.text = WaveTime.ToString();

        // Start coroutines
        coroutine = SpawnPrefab();
        StartCoroutine(coroutine);

        coroutine02 = WaveCountDown();
        StartCoroutine(coroutine02);
    }

    IEnumerator WaveCountDown()
    {
        while (true)
        {
            while(WaveTime > 0)
            {
                yield return new WaitForSeconds(1);
                WaveTime--;

                // Update UI
                CountdownText.text = WaveTime.ToString();
            }
            yield return new WaitForSeconds(1);
            WaveTime = WaveDelayMax;

            // Update UI
            CountdownText.text = WaveTime.ToString();
        }
    }

    IEnumerator SpawnPrefab()
    {
        while (true)
        {
            yield return new WaitForSeconds(CurWaveDelay);
            NextWave();
            while (CurGold >= BasicUnitSpawnCost)
            {
                yield return new WaitForSeconds(SpawnDelay);
                // Adjust wave delay due to spawn delay
                CurWaveDelay -= SpawnDelay;

                // Spawn a unit
                if(CurBasicUnits < MinBasicUnits) // Spawn min of basic units
                {
                    // Spawn prefab at this position
                    Instantiate(BasicUnitPrefab, CurSpawnPoint.position, Quaternion.identity);

                    // Deduct cost
                    CurGold -= BasicUnitSpawnCost;

                    // Increase basic unit count
                    CurBasicUnits++;
                }
                else if(CurHeavyUnits < MinHeavyUnits && CurGold >= HeavyUnitSpawnCost) // Spawn min of heavy units
                {
                    // Spawn prefab at this position
                    Instantiate(HeavyUnitPrefab, CurSpawnPoint.position, Quaternion.identity);

                    // Deduct cost
                    CurGold -= HeavyUnitSpawnCost;

                    // Increase heavy unit count
                    CurHeavyUnits++;
                }
                else if (CurGold >= CommanderSpawnCost && CurCommanders < MaxCommanders) // Spawn commander
                {
                    // Spawn prefab at this position
                    Instantiate(CommanderPrefab, CurSpawnPoint.position, Quaternion.identity);

                    // Deduct cost
                    CurGold -= CommanderSpawnCost;

                    // Increase commander count
                    CurCommanders++;
                }
                else if(CurGold >= HeavyUnitSpawnCost) // Spawn heavy unit
                {
                    // Spawn prefab at this position
                    GameObject GO = Instantiate(HeavyUnitPrefab, CurSpawnPoint.position, Quaternion.identity);

                    // Set spawner ref
                    GO.GetComponent<BehaviourTree>().SpawnerRef = this;

                    // Deduct cost
                    CurGold -= HeavyUnitSpawnCost;

                    // Increase heavy unit count
                    CurHeavyUnits++;
                }
                else // Spawn basic unit
                {
                    // Spawn prefab at this position
                    Instantiate(BasicUnitPrefab, CurSpawnPoint.position, Quaternion.identity);

                    // Deduct cost
                    CurGold -= BasicUnitSpawnCost;

                    // Increase basic unit count
                    CurBasicUnits++;
                }                
            }            
        }        
    }

    void NextWave()
    {
        // Increase wave count
        WaveCount++;       
        
        if(WaveCount > 1)
        {
            // Switch spawn point
            CurSpawnPoint = CurSpawnPoint == SpawnPoint01 ? SpawnPoint02 : SpawnPoint01;

            // Reset counts
            CurBasicUnits = 0;
            CurHeavyUnits = 0;

            // Set wave delay
            CurWaveDelay = WaveDelayMax;

            // Increase gold per wave
            GoldPerWave += GoldIncreasePerWave;
        }        
        // Increase gold
        CurGold += GoldPerWave;
                
        // Update UI
        CurrentWaveText.text = WaveCount.ToString();
    }

    internal void CommanderDead()
    {
        CurCommanders = 0;
    }
}
