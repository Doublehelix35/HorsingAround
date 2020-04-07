using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //UI texts
    public Text HealthText;
    public Text GoldText;

    // Mines
    public GoldMine[] GoldMines;
    public int MineUpgradeCost = 500;

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

    // Player
    GameObject PlayerRef;
    string PlayerName;

    // Colour
    public Renderer[] RendsToChange; // Renders to change the (colour) material of
    public Material BlackMat;
    public Material BlueMat;
    public Material BrownMat;
    public Material GreenMat;
    public Material PurpleMat;
    public Material RedMat;
    public Material WhiteMat;
    public Material YellowMat;


    void Start()
    {
        // Set current gold equal to starting gold
        CurGold = StartingGold;

        // Update gold text
        UpdateGoldText(CurGold.ToString());

        // Init player ref
        PlayerRef = GameObject.FindGameObjectWithTag("Player");

        // Load player
        LoadPlayer();
    }

    void LoadPlayer()
    {
        CharacterData data = new CharacterData();

        // Extract data from file
        if (File.Exists(Application.persistentDataPath + "/" + "CharacterData" + ".dat"))
        {
            // Create a binary formatter and open the save file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + "CharacterData" + ".dat", FileMode.Open);

            // Create an object to store information from the file in and then close the file
            data = (CharacterData)bf.Deserialize(file);

            file.Close();
        }
        else
        {
            Debug.Log("Error! Character not found!");
        }

        // Set up player
        PlayerName = data.CharacterName;

        if(data.CharacterSize > 0f)
        {
            PlayerRef.transform.localScale = new Vector3(data.CharacterSize, data.CharacterSize, data.CharacterSize);
        }

        switch (data.CharacterStatBoost)
        {
            case CharacterCreation.StatBoosts.Attack:
                PlayerRef.GetComponent<Player_Attack>().ApplyAttackBoost();
                break;
            case CharacterCreation.StatBoosts.Health:
                PlayerRef.GetComponent<Player_Health>().ApplyHealthBoost();
                break;
            case CharacterCreation.StatBoosts.Speed:
                PlayerRef.GetComponent<Player_Move>().ApplySpeedBoost();
                break;
        }

        // Player colour change
        // Loop through materials to change and set to chosen colour
        for (int j = 0; j < RendsToChange.Length; j++)
        {
            switch (data.CharacterColour)
            {
                case CharacterCreation.Colours.Black:
                    RendsToChange[j].material = BlackMat;
                    break;
                case CharacterCreation.Colours.Blue:
                    RendsToChange[j].material = BlueMat;
                    break;
                case CharacterCreation.Colours.Brown:
                    RendsToChange[j].material = BrownMat;
                    break;
                case CharacterCreation.Colours.Green:
                    RendsToChange[j].material = GreenMat;
                    break;
                case CharacterCreation.Colours.Purple:
                    RendsToChange[j].material = PurpleMat;
                    break;
                case CharacterCreation.Colours.Red:
                    RendsToChange[j].material = RedMat;
                    break;
                case CharacterCreation.Colours.White:
                    RendsToChange[j].material = WhiteMat;
                    break;
                case CharacterCreation.Colours.Yellow:
                    RendsToChange[j].material = YellowMat;
                    break;
                default:
                    Debug.Log("Didnt change colour for " + RendsToChange[j].name);
                    break;
            }
        }

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

    internal Transform PlaceWorkerInMine(BT_Miner miner)
    {
        Transform selectedMine = null;

        // Place unit in a mine
        for(int i = 0; i < GoldMines.Length; i++)
        {
            // Check if the mine has space
            if (!GoldMines[i].IsCapacityFull())
            {
                // Add worker to mine
                GoldMines[i].AddWorker(miner);

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

    internal void UpgradeAllMines()
    {
        // Minus cost of upgrade
        CurGold -= MineUpgradeCost;

        // Update gold text
        UpdateGoldText(CurGold.ToString());

        for (int i = 0; i < GoldMines.Length; i++)
        {
            // Increase mine's level
            GoldMines[i].IncreaseMineLevel();
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

    public int GetCurrentGold()
    {
        return CurGold;
    }
}
