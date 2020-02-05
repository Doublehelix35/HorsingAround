﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    // Target to follow
    public Transform Target;
    public float RotationSpeed = 1f;

    public Vector3 OffsetMagnitude; // How far behind the player should it be

    GameObject Temp;

    void Start()
    {
        Temp = new GameObject("TempCreatedByCamera");
    }

    void FixedUpdate()
    {
        // Get behind player
        Vector3 behindVec = Target.position - (Target.position + Target.forward);

        // Multiply behind vec by magnitude
        behindVec = new Vector3(behindVec.x * OffsetMagnitude.x, OffsetMagnitude.y, behindVec.z * OffsetMagnitude.z);

        // Set position = target position + offset
        Vector3 newPos = Target.position + behindVec;
        transform.position = newPos;

        // Set target rotation
        Vector3 targetRot = Target.position - behindVec;

        // Use temp game object to store look at rotation
        Temp.transform.position = transform.position;
        Temp.transform.LookAt(targetRot);

        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Lerp(transform.rotation, Temp.transform.rotation, RotationSpeed);
    }
}
