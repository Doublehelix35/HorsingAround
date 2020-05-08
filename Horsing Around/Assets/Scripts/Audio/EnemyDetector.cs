using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    List<GameObject> EnemiesDetected;
    SoundManager SoundManagerRef;
    float EnemyDetectedVolume = 1f;
    float NormalVolume = 0.5f;

    float NullCheckDelay = 0.2f;

    IEnumerator coroutine;

    void Start()
    {
        // Init
        SoundManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<SoundManager>();
        EnemiesDetected = new List<GameObject>();

        // Start null check
        coroutine = NullCheck();
        StartCoroutine(coroutine);

        // Set volume
        SoundManagerRef.SetMusicVolumeModifier(NormalVolume);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            EnemiesDetected.Add(other.gameObject);

            // Only do once
            if(EnemiesDetected.Count == 1)
            {
                SoundManagerRef.SetMusicVolumeModifier(EnemyDetectedVolume);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemiesDetected.Remove(other.gameObject);

            // Only do once
            if (EnemiesDetected.Count == 0)
            {
                SoundManagerRef.SetMusicVolumeModifier(NormalVolume);
            }
        }
    }

    IEnumerator NullCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(NullCheckDelay);

            for(int i = 0; i < EnemiesDetected.Count; i++)
            {
                if(EnemiesDetected[i] == null)
                {
                    EnemiesDetected.Remove(EnemiesDetected[i]);

                    if (EnemiesDetected.Count == 0)
                    {
                        SoundManagerRef.SetMusicVolumeModifier(NormalVolume);
                    }
                }
            }

        }
    }
}
