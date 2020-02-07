using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Cloud.Dialogflow.V2;
using System;
using UnityEngine.UI;

public class DialogFlow : MonoBehaviour
{
    string AIGuideProjectId = "aiguide-ufjadt";
    string AIGuideKeyPath = "Assets/Resources/AIGuideKey/aiguide-ufjadt-d24e69e401d6.json";
    string SessionId = "11234544";
    string LanguageCode = "en-US";

    string PlayerText = "Who are you?";

    public InputField InputBox;
    public Text OutputText;

    public bool isReady = true;


    private void Start()
    {
        // Set environment variable
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AIGuideKeyPath);    
    }

    void Update()
    {
        if (isReady)
        {
            if (Input.GetKeyUp(KeyCode.Return) && InputBox.text != null)
            {
                // Detect intent from text
                string[] text = { InputBox.text };
                DetectIntentFromTexts(AIGuideProjectId, SessionId, text, LanguageCode);
            }
        }        
    }

    public void DetectIntentFromTexts(string projectId, string sessionId, string[] texts, string languageCode = "en-US")
    {
        SessionsClient client = SessionsClient.Create();

        string outputText = "";

        foreach (string text in texts)
        {
            DetectIntentResponse response = client.DetectIntent(
                new SessionName(projectId, sessionId), 
                new QueryInput()
                {
                    Text = new TextInput()
                    {
                        Text = text,
                        LanguageCode = languageCode
                    }
                }
            );

            QueryResult queryResult = response.QueryResult;

            //if (queryResult.Intent != null)
            //{
            //    Debug.Log($"Intent detected: {queryResult.Intent.DisplayName}");
            //}
            //Debug.Log($"Intent confidence: {queryResult.IntentDetectionConfidence}");

            outputText += "You: " + queryResult.QueryText + "\n\n";
            outputText += "Guide: " + queryResult.FulfillmentText + "\n\n";
        }

        // Update output text
        OutputText.text = outputText;
    }
}
