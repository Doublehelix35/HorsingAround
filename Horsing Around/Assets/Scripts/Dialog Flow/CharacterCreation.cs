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

    // Texts
    public Text OutputText;
    public Text ColourText;
    public Text SizeText;
    public Text StatBoostText;
    public Text CharacterNameText;

    // Colour
    public enum Colours { Default, Black, Blue, Brown, Green, Purple, Red, White, Yellow };
    public Colours ChosenColour = Colours.Default;
    public Renderer[] MatsToChange; // Renders to change the (colour) material of
    public Material BlackMat;
    public Material BlueMat;
    public Material BrownMat;
    public Material GreenMat;
    public Material PurpleMat;
    public Material RedMat;
    public Material WhiteMat;
    public Material YellowMat;

    // Size
    public enum Sizes { Small, Default, Large };
    public Sizes ChosenSize = Sizes.Default;
    public float SmallScale = 0.8f;
    public float DefaultScale = 1f;
    public float LargeScale = 1.2f;
    public GameObject PlayerRef;

    // Stat boost
    public enum StatBoosts { Attack, Health, Speed };
    public StatBoosts ChosenStatBoost = StatBoosts.Health;

    // Name
    string CharacterName;
    

    void Start()
    {
        // Set session id to random number
        SessionId = Random.Range(10000, 99999999).ToString();

        // Set environment variable
        System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", CharacterCreationKeyPath);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && InputBox.text != null)
        {
            // Detect intent from text
            string[] text = { InputBox.text };
            DetectIntentFromTexts(CharacterCreationProjectId, SessionId, text);
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
                // Change colour
                case DialogFlow.IntentTypes.ChangeColour:

                    // Loop through parameters
                    foreach (Google.Protobuf.WellKnownTypes.Struct p in parameters)
                    {
                        // Loop through the parameter's values
                        foreach (Google.Protobuf.WellKnownTypes.Value v in p.Fields.Values)
                        {
                            Debug.Log(v.StringValue);

                            switch (v.StringValue)
                            {
                                // Set chosen colour
                                case "black":
                                    ChosenColour = Colours.Black;
                                    break;
                                case "blue":
                                    ChosenColour = Colours.Blue;
                                    break;
                                case "brown":
                                    ChosenColour = Colours.Brown;
                                    break;
                                case "green":
                                    ChosenColour = Colours.Green;
                                    break;                                
                                case "purple":
                                    ChosenColour = Colours.Purple;
                                    break;
                                case "red":
                                    ChosenColour = Colours.Red;
                                    break;
                                case "white":
                                    ChosenColour = Colours.White;
                                    break;
                                case "yellow":
                                    ChosenColour = Colours.Yellow;
                                    break;

                                default:
                                    Debug.Log("Colour not found");
                                    outputText = "Guide: " + "Colour not available";
                                    break;
                            }
                        }

                        // Set colour text
                        ColourText.text = ChosenColour.ToString();

                        // Loop through materials to change and set to chosen colour
                        for (int j = 0; j < MatsToChange.Length; j++)
                        {
                            switch (ChosenColour)
                            {
                                case Colours.Black:
                                    MatsToChange[j].material = BlackMat;
                                    break;
                                case Colours.Blue:
                                    MatsToChange[j].material = BlueMat;
                                    break;
                                case Colours.Brown:
                                    MatsToChange[j].material = BrownMat;
                                    break;
                                case Colours.Green:
                                    MatsToChange[j].material = GreenMat;
                                    break;
                                case Colours.Purple:
                                    MatsToChange[j].material = PurpleMat;
                                    break;
                                case Colours.Red:
                                    MatsToChange[j].material = RedMat;
                                    break;
                                case Colours.White:
                                    MatsToChange[j].material = WhiteMat;
                                    break;
                                case Colours.Yellow:
                                    MatsToChange[j].material = YellowMat;
                                    break;
                                default:
                                    Debug.Log("Didnt change colour for " + MatsToChange[j].name);
                                    break;
                            }
                        }
                    }
                    break;

                // Change size
                case DialogFlow.IntentTypes.ChangeSize:

                    // Loop through parameters
                    foreach (Google.Protobuf.WellKnownTypes.Struct p in parameters)
                    {
                        // Loop through the parameter's values
                        foreach (Google.Protobuf.WellKnownTypes.Value v in p.Fields.Values)
                        {
                            Debug.Log(v.StringValue);

                            switch (v.StringValue)
                            {
                                // Set chosen size
                                case "Small":
                                    ChosenSize = Sizes.Small;
                                    PlayerRef.transform.localScale = new Vector3(SmallScale, SmallScale, SmallScale);
                                    break;
                                case "Normal":
                                    ChosenSize = Sizes.Default;
                                    PlayerRef.transform.localScale = new Vector3(DefaultScale, DefaultScale, DefaultScale);
                                    break;
                                case "Large":
                                    ChosenSize = Sizes.Large;
                                    PlayerRef.transform.localScale = new Vector3(LargeScale, LargeScale, LargeScale);
                                    break;

                                default:
                                    Debug.Log("Size not found");
                                    outputText = "Guide: " + "Size not found";
                                    break;
                            }
                        }

                        // Set size text
                        SizeText.text = ChosenSize.ToString();
                    }
                    break;

                // Change stat boost
                case DialogFlow.IntentTypes.ChangeStatBoost:

                    // Loop through parameters
                    foreach (Google.Protobuf.WellKnownTypes.Struct p in parameters)
                    {
                        // Loop through the parameter's values
                        foreach (Google.Protobuf.WellKnownTypes.Value v in p.Fields.Values)
                        {
                            Debug.Log(v.StringValue);

                            switch (v.StringValue)
                            {
                                // Set chosen stat boost
                                case "Attack":
                                    ChosenStatBoost = StatBoosts.Attack;
                                    break;
                                case "Health":
                                    ChosenStatBoost = StatBoosts.Health;
                                    break;
                                case "Speed":
                                    ChosenStatBoost = StatBoosts.Speed;
                                    break;

                                default:
                                    Debug.Log("Stat boost not found");
                                    outputText = "Guide: " + "Stat boost not found";
                                    break;
                            }
                        }

                        // Set stat boost text
                        StatBoostText.text = ChosenStatBoost.ToString();
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
