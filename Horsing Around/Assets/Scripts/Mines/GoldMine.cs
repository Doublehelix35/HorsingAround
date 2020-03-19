using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    public GameManager GameManagerRef;
    public Material EmptyMat; // Material to show slot is empty
    public Material FullMat; // Material to show slot is full

    public Renderer[] WorkerRenderers; // Array of worker renderers for UI
    
    int GoldPerSecondPerWorker = 1;
    int MaxWorkers = 5;
    int CurNumOfWorkers = 0; // Current number of workers

    float TickDuration = 5; // How long til the next tick in seconds

    IEnumerator coroutine;

    void Start()
    {
        // Start mine gold coroutine
        coroutine = MineGold();
        StartCoroutine(coroutine);
    }

    internal bool IsCapacityFull()
    {
        // Return true if at maximum workers
        return CurNumOfWorkers >= MaxWorkers ? true : false;
    }

    internal void AddWorker()
    {
        if(CurNumOfWorkers < MaxWorkers)
        {
            // Increase worker count
            CurNumOfWorkers++;

            // Set worker UI to the full material
            WorkerRenderers[CurNumOfWorkers - 1].material = FullMat;
        }
        else
        {
            Debug.Log("Mine at worker capacity!");
        }        
    }

    internal void RemoveWorker()
    {
        if(CurNumOfWorkers > 0)
        {
            // Decrease worker count
            CurNumOfWorkers--;

            // Set worker UI to the empty material
            WorkerRenderers[CurNumOfWorkers - 1].material = EmptyMat;
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

            }            
        }
    }
}
