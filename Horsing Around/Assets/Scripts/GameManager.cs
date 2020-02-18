using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text HealthText;
    public Text GoldText;

    public GameObject WorkerPrefab;
    public Transform WorkerSpawn;
    public int WorkerCost = 50;

    public GameObject InfantryPrefab;
    public Transform InfantrySpawn;
    public int InfantryCost = 100;

    public int StartingGold = 1000;
    int CurGold;

    private void Start()
    {
        // Set current gold equal to starting gold
        CurGold = StartingGold;

        // Update gold text
        UpdateGoldText(CurGold.ToString());
    }

    void ChangeCurrentGold(int value)
    {
        // Add value to current gold
        CurGold += value;
    }

    public void SpawnWorkerUnit()
    {
        // Check player can afford it
        if (CurGold >= WorkerCost)
        {
            // Spawn worker
            GameObject GO = Instantiate(WorkerPrefab, WorkerSpawn.position, Quaternion.identity);

            // Deduct worker cost from current gold
            ChangeCurrentGold(-WorkerCost);

            // Update gold text
            UpdateGoldText(CurGold.ToString());
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
}
