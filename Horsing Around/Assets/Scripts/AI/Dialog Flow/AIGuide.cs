using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIGuide : DialogFlow
{
    // Ai guide dialog flow data
    string AIGuideProjectId = "aiguide-ufjadt";
    string AIGuideKeyPath = "Assets/Resources/AIGuideKey/aiguide-ufjadt-d24e69e401d6.json";
    string SessionId;

    // Refs
    public GameObject AIGuidePanel;
    GameManager GameManagerRef;
    public InputField InputBox;
    public Text OutputText;
    public Text DifficultyText;

    bool isReady = false;
    public enum Difficulties { Easy, Normal, Hard, Extreme};
    public Difficulties Difficulty = Difficulties.Normal;

    void Start()
    {
        // Set session id to random number
        SessionId = Random.Range(10000, 99999999).ToString();

        // Set environment variable
        System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AIGuideKeyPath);

        // Init GM ref
        GameManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Turn off panel
        AIGuidePanel.SetActive(false);

        // Make inactive
        isReady = false;

        // Set difficulty text
        DifficultyText.text = Difficulty.ToString();
    }

    void Update()
    {
        if (isReady)
        {
            if (Input.GetKeyUp(KeyCode.Return) && InputBox.text != null)
            {
                // Detect intent from text
                string[] text = { InputBox.text };
                DetectIntentFromTexts(AIGuideProjectId, SessionId, text);
            }
        }
    }

    override protected void ProcessIntents(List<IntentTypes> intents, string outputText, List<Google.Protobuf.WellKnownTypes.Struct> parameters)
    {
        // Handle all incoming intents
        foreach (IntentTypes i in intents)
        {
            // Find the intent's type
            switch (i)
            {
                case IntentTypes.UpgradeMine:
                    if(GameManagerRef.GetCurrentGold() >= GameManagerRef.MineUpgradeCost)
                    {
                        GameManagerRef.UpgradeAllMines();
                    }
                    else
                    {
                        outputText = "Guide: We don't have enough gold to upgrade the mines :(";
                    }
                    
                    break;

                // Change difficulty
                case IntentTypes.ChangeDifficulty:

                    // Loop through parameters
                    foreach (Google.Protobuf.WellKnownTypes.Struct p in parameters)
                    {
                        // Loop through the parameter's values
                        foreach (Google.Protobuf.WellKnownTypes.Value v in p.Fields.Values)
                        {
                            switch (v.StringValue)
                            {
                                case "Easy":
                                    Difficulty = Difficulties.Easy;
                                    break;
                                case "Normal":
                                    Difficulty = Difficulties.Normal;
                                    break;
                                case "Hard":
                                    Difficulty = Difficulties.Hard;
                                    break;
                                case "Extreme":
                                    Difficulty = Difficulties.Extreme;
                                    break;
                                default:
                                    Debug.Log("Difficulty not found");
                                    outputText = "Guide: Difficulty not found";
                                    break;
                            }
                        }

                        // Set difficulty text
                        DifficultyText.text = Difficulty.ToString();
                    }
                    break;

                default:
                    Debug.Log("Intent name not found");
                    break;
            }
        }

        // Update output text
        OutputText.text = outputText;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            // Turn on panel
            AIGuidePanel.SetActive(true);

            // Make active
            isReady = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Turn off panel
            AIGuidePanel.SetActive(false);

            // Make inactive
            isReady = false;
        }
    }
}
