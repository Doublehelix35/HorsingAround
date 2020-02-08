using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Google.Cloud.Dialogflow.V2;

public class AIGuide : MonoBehaviour
{
    // Ai guide dialog flow data
    string AIGuideProjectId = "aiguide-ufjadt";
    string AIGuideKeyPath = "Assets/Resources/AIGuideKey/aiguide-ufjadt-d24e69e401d6.json";
    string SessionId = "11234544";
    string LanguageCode = "en-US";

    // Refs
    public DialogFlow DialogFlowRef;
    public GameObject AIGuidePanel;
    public InputField InputBox;
    public Text OutputText;
    public Text DifficultyText;

    bool isReady = false;
    public enum Difficulties { Easy, Normal, Hard, Extreme};
    public Difficulties Difficulty = Difficulties.Normal;

    void Start()
    {
        // Set environment variable
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AIGuideKeyPath);

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
                DialogFlowRef.DetectIntentFromTexts(AIGuideProjectId, SessionId, text, LanguageCode);
            }
        }
    }

    internal void ProcessIntents(List<DialogFlow.IntentTypes> intents, string outputText, List<Google.Protobuf.WellKnownTypes.Struct> parameter)
    {
        // Handle all incoming intents
        foreach(DialogFlow.IntentTypes i in intents)
        {
            switch (i)
            {
                case DialogFlow.IntentTypes.ChangeDifficulty:
                    // Change difficulty
                    

                    //if (parameters == "Difficulty")
                    //{
                    //    Debug.Log(p.Value);
                    //    // Set difficult equal to value
                    //    switch (p.Value)
                    //    {
                    //        case "Easy":
                    //            Difficulty = Difficulties.Easy;
                    //            break;
                    //        case "Normal":
                    //            Difficulty = Difficulties.Normal;
                    //            break;
                    //        case "Hard":
                    //            Difficulty = Difficulties.Hard;
                    //            break;
                    //        case "Extreme":
                    //            Difficulty = Difficulties.Extreme;
                    //            break;
                    //        default:
                    //            Debug.Log("Difficulty not found");
                    //            break;
                    //    }

                    //    // Set difficulty text
                    //    DifficultyText.text = Difficulty.ToString();
                    //}
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
