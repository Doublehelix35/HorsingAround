using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    // Target to follow
    public Transform Target;
    public Vector3 PositionOffset;
    public float RotationSpeed = 1f;


    public float OffsetMagnitude = 1f; // How far behind the player should it be

    float RotationX = 20f;

    Vector3 NewPos;

    void Start()
    {
        NewPos = new Vector3(Target.position.x + PositionOffset.x, Target.position.y + PositionOffset.y, Target.position.z + PositionOffset.z);
        transform.position = NewPos;
    }

    void Update()
    {
        // Set position = target position + offset
        NewPos = new Vector3(Target.position.x + PositionOffset.x, Target.position.y + PositionOffset.y, Target.position.z + PositionOffset.z);
        transform.position = NewPos;

        //// Get behind player
        //Vector3 behindVec = RailsToFollow.position - CenterRef.position;

        //// Multiply behind vec by magnitude
        //behindVec = new Vector3(behindVec.x * OffsetMagnitude, behindVec.y * OffsetMagnitude, behindVec.z * OffsetMagnitude);

        //// Set position = rails position + offset
        //transform.position = new Vector3(RailsToFollow.position.x + behindVec.x, RailsToFollow.position.y + behindVec.y, RailsToFollow.position.z + behindVec.z);

        //// Camera shouldnt be below rails
        //if (RailsToFollow.position.y > transform.position.y)
        //{
        //    // Calc distance on y axis from rails
        //    float distY = RailsToFollow.position.y - transform.position.y;

        //    // Set new position
        //    transform.position = new Vector3(transform.position.x + OffsetMagnitude, transform.position.y + distY, transform.position.z + OffsetMagnitude);
        //}

        //// Face the center
        //transform.LookAt(CenterRef);
    }
}
