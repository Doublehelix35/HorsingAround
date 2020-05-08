using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterRotate : MonoBehaviour
{
    public float RotateSpeed = 10f; // Speed the player rotates around center of sphere 
    float CurrentRotation = 0f;
    public bool UpAxisOnly = false;

    void Update()
    {
        // Update current rotation
        CurrentRotation = RotateSpeed * Time.deltaTime;

        if (!UpAxisOnly)
        {
            // Rotate around center based on forward axis
            transform.RotateAround(transform.position, transform.forward, CurrentRotation);
        }        

        // Rotate around center based on up axis
        transform.RotateAround(transform.position, transform.up, CurrentRotation);
    }
}
