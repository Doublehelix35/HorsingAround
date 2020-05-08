using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource MusicSource;
    public Slider VolumeSlider;

    float VolumeCurrent = 1f;
    float VolumeMax = 1f;
    float VolumeModifier = 1f;


    void Start()
    {
        LoadSoundSettings();
    }

    internal void SetMusicVolumeModifier(float volumeModifier)
    {
        // Clamp between 0 and 1
        VolumeModifier = Mathf.Clamp01(volumeModifier);

        UpdateMusicVolume();
    }

    void UpdateMusicVolume()
    {
        VolumeCurrent = VolumeMax * VolumeModifier;
        MusicSource.volume = VolumeCurrent;
    }

    public void AdjustVolumeMax(float newVolume)
    {
        VolumeMax = Mathf.Clamp01(newVolume);

        UpdateMusicVolume();
    }

    public void SaveSoundSettings()
    {
        // Create a binary formatter and a new file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + "SoundSettings" + ".dat");

        // Create an object to save information to
        SoundData data = new SoundData();

        // Save sound settings
        data.CurrentVolume = VolumeCurrent;
        data.MaxVolume = VolumeMax;
        data.VolumeModifier = VolumeModifier;

        // Write the object to file and close it
        bf.Serialize(file, data);
        file.Close();
    }

    void LoadSoundSettings()
    {
        if (File.Exists(Application.persistentDataPath + "/" + "SoundSettings" + ".dat"))
        {
            // Create a binary formatter and open the save file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + "SoundSettings" + ".dat", FileMode.Open);

            // Create an object to store information from the file in and then close the file
            SoundData data = new SoundData();
            data = (SoundData)bf.Deserialize(file);

            VolumeCurrent = data.CurrentVolume;
            VolumeMax = data.MaxVolume;
            VolumeModifier = data.VolumeModifier;

            if(VolumeSlider != null)
            {
                VolumeSlider.value = VolumeCurrent;
            }

            file.Close();
        }
        else
        {
            Debug.Log("Error! Sound settings not found!");
        }
    }
}

[System.Serializable]
class SoundData
{
    public float CurrentVolume;
    public float MaxVolume;
    public float VolumeModifier;
}
