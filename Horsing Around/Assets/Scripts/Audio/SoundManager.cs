using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource MusicSource;

    float VolumeCurrent = 1f;
    float VolumeMax = 1f;
    float VolumeModifier = 1f;

    
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
}
