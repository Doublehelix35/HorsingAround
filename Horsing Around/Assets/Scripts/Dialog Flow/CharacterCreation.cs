using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : DialogFlow
{
    // Character creation dialog flow data
    string CharacterCreationProjectId = "charactercreation-ybetwt";
    string CharacterCreationKeyPath = "Assets/Resources/CharacterCreationKey/charactercreation-ybetwt-0e59ee527322.json";
    string SessionId;

    // Refs
    public GameObject CharacterCreationPanel;
    public InputField InputBox;
    public Text OutputText;
    public Text DifficultyText;

    bool isReady = false;
    public enum Difficulties { Easy, Normal, Hard, Extreme };
    public Difficulties Difficulty = Difficulties.Normal;

    void Start()
    {
        // Set session id to random number
        SessionId = Random.Range(10000, 99999999).ToString();

        // Set environment variable
        System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", CharacterCreationKeyPath);

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
                DetectIntentFromTexts(CharacterCreationProjectId, SessionId, text);
            }
        }
    }

    override protected void ProcessIntents(List<DialogFlow.IntentTypes> intents, string outputText, List<Google.Protobuf.WellKnownTypes.Struct> parameters)
    {
        // Handle all incoming intents
        foreach (DialogFlow.IntentTypes i in intents)
        {
            // Find the intent's type
            switch (i)
            {
                // Change difficulty
                case DialogFlow.IntentTypes.ChangeDifficulty:

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
}
