using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Main Camera
    Camera MainCamera;

    // UI texts
    public Text HealthText;
    public Text GoldText;
    public Text NameText;
    public Text MineUpgradeText;
    public Text UnitsUpgradeText;

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
    int MaxWorkers = 25;
    int WorkerCount = 0;

    // Patrol Spots
    public Transform[] PatrolSpots;
    int CurrentPatrolSpot = -1;

    // Infantry to spawn
    public GameObject InfantryPrefab;
    public Transform InfantrySpawn;
    public int InfantryCost = 100;

    // Unit upgrades
    public enum UnitUpgradeStages { Stage1, Stage2, Stage3}; // 1 = wood elves, 2 = Dark elves, 3 = High elves
    UnitUpgradeStages CurrentUnitStage = UnitUpgradeStages.Stage1;
    public int UnitUpgradeCostStage2 = 1000;
    public int UnitUpgradeCostStage3 = 3000;

    // Gold
    public int StartingGold = 1000;
    int CurGold;

    // Player
    GameObject PlayerRef;
    string PlayerName;

    // Optimiser
    Optimiser OptimiserRef;

    // Colour
    public Renderer[] RendsToChange; // Renders to change the (colour) material of
    CharacterCreation.Colours PlayerColour;
    public Material[] BlackMat = new Material[3];
    public Material[] BlueMat = new Material[3];
    public Material[] BrownMat = new Material[3];
    public Material[] GreenMat = new Material[3];
    public Material[] PurpleMat = new Material[3];
    public Material[] RedMat = new Material[3];
    public Material[] WhiteMat = new Material[3];
    public Material[] YellowMat = new Material[3];

    // Map
    public GameObject MapCamera;
    bool IsMapActive = false;

    // Game over
    public GameObject GameOverPanel;
    bool isPlayerDead = false;
    float GameOverDelay = 1f;
    float DeathTime;
    bool GameOverOnce = false;

    // Pause game
    public GameObject PausePanel;

    // Ai guide
    public Text OutputText;
    bool isGuideActive = false;


    void Start()
    {
        // Init camera
        MainCamera = Camera.main;

        // Set current gold equal to starting gold
        CurGold = StartingGold;

        // Update gold text
        UpdateGoldText(CurGold.ToString());

        // Init player ref
        PlayerRef = GameObject.FindGameObjectWithTag("Player");

        // Load player
        LoadPlayer();

        // Init optimiser ref
        OptimiserRef = PlayerRef.transform.GetComponentInChildren<Optimiser>();
    }

    void Update()
    {
        // Input
        if (!isGuideActive)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                // Toggle map visibility
                IsMapActive = !IsMapActive;
                MapCamera.SetActive(IsMapActive);
                MainCamera.enabled = !IsMapActive; // Turn main off if map camera is on          
            }

            // Pause game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
                PausePanel.SetActive(true);
            }
        }        

        // Game over
        if (isPlayerDead && Time.time >= DeathTime + GameOverDelay && !GameOverOnce)
        {
            // Show gameover panel
            GameOverPanel.SetActive(true);

            // Pause time
            Time.timeScale = 0f;

            // Make sure above is only done once
            GameOverOnce = true;
        }
    }

    /*/ Player methods /*/

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

        // Update name UI
        NameText.text = PlayerName;

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
            PlayerColour = data.CharacterColour;
            switch (data.CharacterColour)
            {
                case CharacterCreation.Colours.Black:
                    RendsToChange[j].material = BlackMat[0];
                    break;
                case CharacterCreation.Colours.Blue:
                    RendsToChange[j].material = BlueMat[0];
                    break;
                case CharacterCreation.Colours.Brown:
                    RendsToChange[j].material = BrownMat[0];
                    break;
                case CharacterCreation.Colours.Green:
                    RendsToChange[j].material = GreenMat[0];
                    break;
                case CharacterCreation.Colours.Purple:
                    RendsToChange[j].material = PurpleMat[0];
                    break;
                case CharacterCreation.Colours.Red:
                    RendsToChange[j].material = RedMat[0];
                    break;
                case CharacterCreation.Colours.White:
                    RendsToChange[j].material = WhiteMat[0];
                    break;
                case CharacterCreation.Colours.Yellow:
                    RendsToChange[j].material = YellowMat[0];
                    break;
                case CharacterCreation.Colours.Default:
                    RendsToChange[j].material = GreenMat[0];
                    break;
                default:
                    Debug.Log("Didnt change colour for " + RendsToChange[j].name + " " + data.CharacterColour);
                    break;
            }
        }
    }

    public void PlayerDead()
    {
        isPlayerDead = true;

        // Set death time
        DeathTime = Time.time;        
    }

    /*/ Gold methods /*/

    internal void ChangeCurrentGold(int value)
    {
        // Add value to current gold
        CurGold += value;

        // Update gold text
        UpdateGoldText(CurGold.ToString());
    }

    /*/ Unit management methods /*/

    public void SpawnWorkerUnit()
    {
        // Check player can afford it and not at max workers
        if (CurGold >= WorkerCost && WorkerCount < MaxWorkers)
        {
            // Spawn worker
            GameObject GO = Instantiate(WorkerPrefab, WorkerSpawn.position, Quaternion.identity);

            // Set worker to current stage
            GO.GetComponent<BT_Miner>().UpgradeMiner(CurrentUnitStage);  

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

    internal Transform GiveUnitPatrolSpot()
    {
        // Increase current patrol spot
        CurrentPatrolSpot++;

        // Check current patrol spot still in bounds
        CurrentPatrolSpot = CurrentPatrolSpot >= PatrolSpots.Length ? 0 : CurrentPatrolSpot;

        return PatrolSpots[CurrentPatrolSpot];
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

        for (int i = 0; i < GoldMines.Length; i++)
        {
            // Increase mine's level
            GoldMines[i].IncreaseMineLevel();
        }

        // Update text
        UpdateGoldText(CurGold.ToString());
        MineUpgradeText.text = (GoldMines[0].GetCurLevel() - 1).ToString();
    }

    internal bool UpgradeAllUnits()
    {
        bool isUpgraded = false;
        // Increase unit stage
        switch (CurrentUnitStage)
        {
            case UnitUpgradeStages.Stage1:
                // Increase to stage 2
                CurrentUnitStage = UnitUpgradeStages.Stage2;
                // Deduct upgrade cost
                CurGold -= UnitUpgradeCostStage2;
                // Update text
                UpdateGoldText(CurGold.ToString());
                UnitsUpgradeText.text = "1";
                isUpgraded = true;
                break;

            case UnitUpgradeStages.Stage2:
                // Increase to stage 3
                CurrentUnitStage = UnitUpgradeStages.Stage3;
                // Deduct upgrade cost
                CurGold -= UnitUpgradeCostStage3;
                // Update text
                UpdateGoldText(CurGold.ToString());
                UnitsUpgradeText.text = "2";
                isUpgraded = true;
                break;

            case UnitUpgradeStages.Stage3:
                Debug.Log("Cant upgrade units! Already at max stage");
                return false;

            default:
                break;
        }

        // Update unit materials
        if (isUpgraded)
        {
            if (CurrentUnitStage == UnitUpgradeStages.Stage2)
            {
                for (int i = 0; i < GoldMines.Length; i++)
                {
                    // Increase miner's level
                    GoldMines[i].UpgradeMiners(CurrentUnitStage);
                }

                // Upgrade player
                // Loop through renderers to change and set to player colour
                for (int i = 0; i < RendsToChange.Length; i++)
                {
                    switch (PlayerColour)
                    {
                        case CharacterCreation.Colours.Black:
                            RendsToChange[i].material = BlackMat[1];
                            break;
                        case CharacterCreation.Colours.Blue:
                            RendsToChange[i].material = BlueMat[1];
                            break;
                        case CharacterCreation.Colours.Brown:
                            RendsToChange[i].material = BrownMat[1];
                            break;
                        case CharacterCreation.Colours.Green:
                            RendsToChange[i].material = GreenMat[1];
                            break;
                        case CharacterCreation.Colours.Purple:
                            RendsToChange[i].material = PurpleMat[1];
                            break;
                        case CharacterCreation.Colours.Red:
                            RendsToChange[i].material = RedMat[1];
                            break;
                        case CharacterCreation.Colours.White:
                            RendsToChange[i].material = WhiteMat[1];
                            break;
                        case CharacterCreation.Colours.Yellow:
                            RendsToChange[i].material = YellowMat[1];
                            break;
                        default:
                            Debug.Log("Error! Didnt change colour for player");
                            break;
                    }
                }
                // Return successful
                return true;
            }
            else // Upgraded to stage 3
            {
                for (int i = 0; i < GoldMines.Length; i++)
                {
                    // Increase miner's level
                    GoldMines[i].UpgradeMiners(CurrentUnitStage);

                }

                // Upgrade player
                // Loop through renderers to change and set to player colour
                for (int i = 0; i < RendsToChange.Length; i++)
                {
                    switch (PlayerColour)
                    {
                        case CharacterCreation.Colours.Black:
                            RendsToChange[i].material = BlackMat[2];
                            break;
                        case CharacterCreation.Colours.Blue:
                            RendsToChange[i].material = BlueMat[2];
                            break;
                        case CharacterCreation.Colours.Brown:
                            RendsToChange[i].material = BrownMat[2];
                            break;
                        case CharacterCreation.Colours.Green:
                            RendsToChange[i].material = GreenMat[2];
                            break;
                        case CharacterCreation.Colours.Purple:
                            RendsToChange[i].material = PurpleMat[2];
                            break;
                        case CharacterCreation.Colours.Red:
                            RendsToChange[i].material = RedMat[2];
                            break;
                        case CharacterCreation.Colours.White:
                            RendsToChange[i].material = WhiteMat[2];
                            break;
                        case CharacterCreation.Colours.Yellow:
                            RendsToChange[i].material = YellowMat[2];
                            break;
                        default:
                            Debug.Log("Error! Didnt change colour for player");
                            break;
                    }
                }
                // Return successful
                return true;
            }
        }
        // Return unsuccessful
        return false;
    }

    internal int GetUnitUpgradeCost()
    {
        switch (CurrentUnitStage)
        {
            case UnitUpgradeStages.Stage1:
                return UnitUpgradeCostStage2;
            case UnitUpgradeStages.Stage2:
                return UnitUpgradeCostStage3;
            case UnitUpgradeStages.Stage3:
                return 0;
        }
        return 100000000;
    }
    
    /*/ UI Methods /*/

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

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void ChangeIsGuideActive(bool status)
    {
        isGuideActive = status;

        if (!isGuideActive)
        {
            OutputText.text = "You: ...";
        }
    }
}
