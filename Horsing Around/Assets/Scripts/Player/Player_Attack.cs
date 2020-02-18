using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    public float AttackDuration = 0.1f; // How long an attack lasts for

    float LastAttackTime; // Stores the time of the last attack
    BoxCollider AttackCol; // Attack collider on this object
    Animator PlayerAnimator; // Animator on parent

    void Start()
    {
        // Init attack collider and disable it
        AttackCol = GetComponent<BoxCollider>();
        AttackCol.enabled = false;

        // Init Player animator
        PlayerAnimator = transform.parent.GetComponent<Animator>();

        // Init last attack time
        LastAttackTime = Time.time;
    }
    
    void Update()
    {
        if(LastAttackTime + AttackDuration < Time.time)
        {
            // Reset attack
            AttackCol.enabled = false;

            // Set is attacking to false
            PlayerAnimator.SetBool("IsAttacking", false);
        }

        // Attack (only if attack col is disabled)
        if (Input.GetKeyDown(KeyCode.X) && AttackCol.enabled == false) 
        {
            // Turn on attack collider
            AttackCol.enabled = true;

            // Set is attacking to true
            PlayerAnimator.SetBool("IsAttacking", true);

            // Set last attack time
            LastAttackTime = Time.time;
        }        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            // Destroy enemy
            Destroy(col.gameObject);
        }
    }
}
