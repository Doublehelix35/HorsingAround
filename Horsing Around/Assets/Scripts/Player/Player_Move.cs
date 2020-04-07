using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Player_Move : MonoBehaviour
{
    public float RotationSpeed = 1f;
    public float MoveSpeed = 1f;
    public float SpeedBoost = 0.5f;
    public float SprintMultiplier = 2f; // Multiplier for sprint speed
    float SprintMultiplierDefault = 1f; // Default value for multiplier
    float CurrentSprintMultiplier = 1f;

    GameObject CameraRef;
    Rigidbody Rigid;
    Animator Anim;
    
    void Start()
    {
        // Init camera
        if (Camera.main != null)
        {
            CameraRef = Camera.main.gameObject;
        }
        else
        {
            Debug.Log("No main camera found! Player move needs it to work");            
        }

        // Init rigid
        Rigid = GetComponent<Rigidbody>();
        // Init animator
        Anim = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        // Get direction input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical") ;

        // Calc angle velocity
        Vector3 angleVelocity = new Vector3(0f, x * RotationSpeed, 0f);

        // Calc delta rotation
        Quaternion deltaRot = Quaternion.Euler(angleVelocity * Time.deltaTime);

        // Rotate player
        Rigid.MoveRotation(Rigid.rotation * deltaRot);        

        // Calc direction
        Vector3 dir =  z * transform.forward;
        dir = dir.normalized; // Normalize direction (so diagonal isnt faster)

        // Set sprint multiplier
        CurrentSprintMultiplier = Input.GetKey(KeyCode.LeftShift) ? SprintMultiplier : SprintMultiplierDefault;

        // Set speed
        dir = dir * MoveSpeed * CurrentSprintMultiplier; // Apply sprint speed
        dir.y = Rigid.velocity.y; // Dont want the player to change velocity on the y;

        // Set velocity = direction
        Rigid.velocity = dir;    

        // Set current max speed to the velocity magnitude
        float curMaxSpeed = Rigid.velocity.magnitude;

        // Clamp max speed between 0 and 1
        curMaxSpeed = Mathf.Clamp(curMaxSpeed, 0f, 1f);

        // Tell the animator the speed for transitions
        Anim.SetFloat("Speed", curMaxSpeed);

        // Set sprint bool in animator
        bool isSprinting = CurrentSprintMultiplier > SprintMultiplierDefault ? true : false;
        Anim.SetBool("IsSprinting", isSprinting);
    }

    internal void ApplySpeedBoost()
    {
        MoveSpeed += SpeedBoost;
    }
}
