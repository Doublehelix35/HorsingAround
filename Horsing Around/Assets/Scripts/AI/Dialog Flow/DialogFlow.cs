using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Cloud.Dialogflow.V2;

public abstract class DialogFlow : MonoBehaviour
{      
    public enum IntentTypes { ChangeColour, ChangeDifficulty, ChangeSize, ChangeStatBoost, EnemyStory, GuideStory, UpgradeMine, WorldStory };


    protected void DetectIntentFromTexts(string projectId, string sessionId, string[] texts, string languageCode = "en-US")
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

                        //Debug.Log(queryResult.Parameters + " paras");

                        parameters.Add(queryResult.Parameters);
                        
                        break;

                    case "ChangeColour":
                        intents.Add(IntentTypes.ChangeColour);

                        parameters.Add(queryResult.Parameters);
                        break;

                    case "SetSize":
                        intents.Add(IntentTypes.ChangeSize);

                        parameters.Add(queryResult.Parameters);
                        break;

                    case "ChangeStatBoost":
                        intents.Add(IntentTypes.ChangeStatBoost);

                        parameters.Add(queryResult.Parameters);
                        break;

                    case "UpgradeMine":
                        intents.Add(IntentTypes.UpgradeMine);

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

        // Pass data to the subclass for it to process
        ProcessIntents(intents, outputText, parameters);        
    }

    abstract protected void ProcessIntents(List<DialogFlow.IntentTypes> intents, string outputText, List<Google.Protobuf.WellKnownTypes.Struct> parameters);
}
