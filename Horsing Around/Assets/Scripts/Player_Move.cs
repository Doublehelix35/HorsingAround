using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Player_Move : MonoBehaviour
{
    public float RotationSpeed = 1f;
    public float MoveSpeed = 1f;
    public float SprintMultiplier = 2f; // Multiplier for sprint speed
    float SprintMultiplierDefault = 1f; // Default value for multiplier
    float CurrentSprintMultiplier = 1f;

    //public float MinRotationY = 0f;
    //public float MaxRotationY = 45f;

    GameObject CameraRef;
    Rigidbody Rigid;
    Animator Anim;
    

    // Start is called before the first frame update
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
        float x = Input.GetAxis("Horizontal") * -1f;
        float z = Input.GetAxis("Vertical") * -1f;

        // Calc direction
        Vector3 dir = x * Vector3.right + z * Vector3.forward;
        dir = dir.normalized; // Normalize direction (so diagonal isnt faster)

        // Set sprint multiplier
        CurrentSprintMultiplier = Input.GetKey(KeyCode.LeftShift) ? SprintMultiplier : SprintMultiplierDefault;

        // Set speed
        dir = dir * MoveSpeed * CurrentSprintMultiplier; // Apply sprint speed
        dir.y = Rigid.velocity.y; // Dont want the player to change velocity on the y;

        // Set velocity = direction
        Rigid.velocity = dir;

        // Rotate towards velocity
        Vector3 target = transform.position + Rigid.velocity;

        // Set target rotation
        Vector3 targetRot = target - transform.position;
        targetRot.y = 0f;
        //targetRot.y = Mathf.Clamp(targetRot.y, MinRotationY, MaxRotationY);

        // Only rotate if target rotation is not zero
        if(targetRot != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetRot), RotationSpeed);
        }       

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
}
