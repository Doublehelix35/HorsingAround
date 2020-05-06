using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    public Material EmptyMat; // Material to show slot is empty
    public Material FullMat; // Material to show slot is full

    public Renderer[] WorkerRenderers; // Array of worker renderers for UI
    BT_Miner[] Workers;

    // Mine level
    public int MaxMineLevel = 5;
    int CurLevel = 1;
    public float LevelTickModifier = 0.1f; // Amount taken off tick duration every level up

    int GoldPerTickPerWorker = 1;
    int MaxWorkers = 5;
    int CurNumOfWorkers = 0; // Current number of workers
    public float MinMiningDistance = 4f;

    public float TickDuration = 1f; // How long til the next tick in seconds

    IEnumerator coroutine;

    void Start()
    {
        // Init worker array
        Workers = new BT_Miner[MaxWorkers];

        // Start mine gold coroutine
        coroutine = MineGold();
        StartCoroutine(coroutine);
    }

    internal bool IsCapacityFull()
    {
        // Return true if at maximum workers
        return CurNumOfWorkers >= MaxWorkers ? true : false;
    }

    internal void AddWorker(BT_Miner miner)
    {
        if(CurNumOfWorkers < MaxWorkers)
        {
            // Increase worker count
            CurNumOfWorkers++;

            // Add miner to array
            Workers[CurNumOfWorkers - 1] = miner;

            // Set worker UI to the full material
            WorkerRenderers[CurNumOfWorkers - 1].material = FullMat;
        }
        else
        {
            Debug.Log("Mine at worker capacity!");
        }        
    }

    internal void RemoveWorker(BT_Miner miner)
    {
        if(CurNumOfWorkers > 0)
        {
            for(int i = 0; i < Workers.Length; i++)
            {
                if(Workers[i] == miner)
                {
                    // Set to null
                    Workers[i] = null;

                    // Decrease worker count
                    CurNumOfWorkers--;

                    // Set worker UI to the empty material
                    WorkerRenderers[CurNumOfWorkers - 1].material = EmptyMat;
                }
                else
                {
                    Debug.Log("Error! Miner not found in " + gameObject.name);
                }
            }            
        }
        else
        {
            Debug.Log("No workers to remove from mine!");
        }
    }

    IEnumerator MineGold()
    {
        while (true)
        {            
            yield return new WaitForSeconds(TickDuration); // Mine gold every tick

            if (CurNumOfWorkers > 0)
            {
                // Give each worker gold
                for(int i = 0; i < Workers.Length; i++)
                {
                    if(Workers[i] != null)
                    {
                        // Check worker is at mine
                        float dist = Vector3.Distance(transform.position, Workers[i].transform.position);
                        if(dist < MinMiningDistance)
                        {
                            Workers[i].ChangeGold(GoldPerTickPerWorker);
                        }
                    }
                }
            }            
        }
    }   
    
    internal void IncreaseMineLevel()
    {
        if(CurLevel < MaxMineLevel)
        {
            CurLevel++;
            TickDuration -= LevelTickModifier;
        }
        else
        {
            Debug.Log("Couldnt increase mine level " + gameObject.name);
        }
    }

    internal int GetCurLevel()
    {
        return CurLevel;
    }

    internal void UpgradeMiners(GameManager.UnitUpgradeStages stage)
    {
        for (int i = 0; i < Workers.Length; i++)
        {
            if (Workers[i] != null)
            {
                // Upgrade miner
                Workers[i].UpgradeMiner(stage);
            }
        }
    }
}
