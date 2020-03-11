using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int MaxHealth = 10;
    int CurHealth;

    public float ImmunityDuration = 0.5f; // How long immunity lasts for
    float ImmunityStartTime; // Stores the time of the immunity start
    bool IsImmune = false;

    Animator PlayerAnimator; // Animator on this object
    GameManager GameManagerRef;


    void Start()
    {
        // Init Player animator
        PlayerAnimator = GetComponent<Animator>();

        // Find game manager
        GameManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Set current health to max health
        CurHealth = MaxHealth;

        // Init immunity start time
        ImmunityStartTime = Time.time;

        // Update health text
        GameManagerRef.UpdateHealthText(CurHealth.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (ImmunityStartTime + ImmunityDuration < Time.time)
        {
            // Turn off immunity
            IsImmune = false;
        }
    }

    internal void ChangeHealth(int value)
    {
        // Add value to current health
        CurHealth += value;

        // Update health text
        GameManagerRef.UpdateHealthText(CurHealth.ToString());

        if (CurHealth <= 0)
        {
            // Player is dead
            Debug.Log("Player is dead");

            // Set is dead to true
            PlayerAnimator.SetBool("IsDead", true);

            // Disable other scripts
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Enemy" && !IsImmune)
        {
            // Take damage
            ChangeHealth(-1);

            // Trigger damaged animation
            PlayerAnimator.SetTrigger("IsAttacked");

            // Make immune
            IsImmune = true;
            ImmunityStartTime = Time.time;
        }

        if(col.gameObject.tag == "KillZone")
        {
            // Kill player
            ChangeHealth(-1000);
        }
    }
}
