using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //UI texts
    public Text HealthText;
    public Text GoldText;

    // Mines
    public GoldMine[] GoldMines;

    // Houses
    public Transform[] Houses;
    int CurrentHouse = -1;

    // Worker to spawn
    public GameObject WorkerPrefab;
    public Transform WorkerSpawn;
    public int WorkerCost = 50;
    public int MaxWorkers = 25;
    int WorkerCount = 0;

    // Infantry to spawn
    public GameObject InfantryPrefab;
    public Transform InfantrySpawn;
    public int InfantryCost = 100;

    // Gold
    public int StartingGold = 1000;
    int CurGold;

    private void Start()
    {
        // Set current gold equal to starting gold
        CurGold = StartingGold;

        // Update gold text
        UpdateGoldText(CurGold.ToString());
    }

    internal void ChangeCurrentGold(int value)
    {
        // Add value to current gold
        CurGold += value;

        // Update gold text
        UpdateGoldText(CurGold.ToString());
    }

    public void SpawnWorkerUnit()
    {
        // Check player can afford it and not at max workers
        if (CurGold >= WorkerCost && WorkerCount < MaxWorkers)
        {
            // Spawn worker
            GameObject GO = Instantiate(WorkerPrefab, WorkerSpawn.position, Quaternion.identity);            

            // Deduct worker cost from current gold
            ChangeCurrentGold(-WorkerCost);

            // Update gold text
            UpdateGoldText(CurGold.ToString());

            // Increase worker count
            WorkerCount++;      
        }
        else
        {
            Debug.Log("Not enough gold for worker!");
        }
        
    }

    public void SpawnInfantryUnit()
    {
        // Check player can afford it
        if (CurGold >= InfantryCost)
        {
            // Spawn infantry
            GameObject GO = Instantiate(InfantryPrefab, InfantrySpawn.position, Quaternion.identity);

            // Deduct infantry cost from current gold
            ChangeCurrentGold(-InfantryCost);

            // Update gold text
            UpdateGoldText(CurGold.ToString());
        }
        else
        {
            Debug.Log("Not enough gold for infantry!");
        }
    }

    internal Transform PlaceUnitInHouse()
    {
        // Increase current house
        CurrentHouse++;

        // Check current house still in bounds
        CurrentHouse = CurrentHouse >= Houses.Length ? 0 : CurrentHouse;

        //Debug.Log("CH " + CurrentHouse + " L " + Houses.Length);

        return Houses[CurrentHouse];
    }

    internal Transform PlaceWorkerInMine()
    {
        Transform selectedMine = null;

        // Place unit in a mine
        for(int i = 0; i < GoldMines.Length; i++)
        {
            // Check if the mine has space
            if (!GoldMines[i].IsCapacityFull())
            {
                // Add worker to mine
                GoldMines[i].AddWorker();

                // selected mine = child called portal position
                selectedMine = GoldMines[i].transform.Find("PortalPosition");
                break;
            }
            else
            {
                Debug.Log("No room in this mine: " + GoldMines[i].gameObject.name);
            }
        }

        return selectedMine;
    }
    
    void UpdateGoldText(string newTextValue)
    {
        // Set gold text
        GoldText.text = newTextValue;
    }

    public void UpdateHealthText(string newTextValue)
    {
        // Set health text
        HealthText.text = newTextValue;
    }

    public int GetCurrentGold()
    {
        return CurGold;
    }
}
