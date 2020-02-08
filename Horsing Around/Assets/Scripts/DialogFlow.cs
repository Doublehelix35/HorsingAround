﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Cloud.Dialogflow.V2;

public class DialogFlow : MonoBehaviour
{      
    public enum IntentTypes { ChangeDifficulty, EnemyStory, GuideStory, WorldStory };

    AIGuide AIGuideRef;


    private void Start()
    {
        // Init AIGuideRef
        AIGuideRef = GameObject.FindGameObjectWithTag("AIGuide").GetComponent<AIGuide>();
    }    

    internal void DetectIntentFromTexts(string projectId, string sessionId, string[] texts, string languageCode = "en-US")
    {
        SessionsClient client = SessionsClient.Create();

        string outputText = "";
        List<IntentTypes> intents = new List<IntentTypes>();
        List<Google.Protobuf.WellKnownTypes.Struct> parameters = new List<Google.Protobuf.WellKnownTypes.Struct>();

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

            if (queryResult.Intent != null)
            {
                switch (queryResult.Intent.DisplayName)
                {
                    case "ChangeDifficulty":
                        intents.Add(IntentTypes.ChangeDifficulty);

                        Debug.Log(queryResult.Parameters + " paras");

                        parameters.Add(queryResult.Parameters);
                        
                        break;

                    default:
                        Debug.Log("Intent name not found");
                        break;
                }
            }
            //Debug.Log($"Intent confidence: {queryResult.IntentDetectionConfidence}");

            outputText += "You: " + queryResult.QueryText + "\n\n";
            outputText += "Guide: " + queryResult.FulfillmentText + "\n\n";
        }

        // Pass data to Ai guide for it to process
        AIGuideRef.ProcessIntents(intents, outputText, parameters);        
    }
}
