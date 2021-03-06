﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int MaxHealth = 10;
    public int HealthBoost = 5;
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
        // Dont take damage if immune
        if (!IsImmune)
        {
            // Add value to current health
            CurHealth += value;            

            if (CurHealth <= 0)
            {
                // Player is dead
                Debug.Log("Player is dead");

                // Set is dead to true
                PlayerAnimator.SetBool("IsDead", true);

                // Update game manager
                GameManagerRef.PlayerDead();
            }
            else if(CurHealth > MaxHealth)
            {
                CurHealth = MaxHealth;
            }

            // Update health text
            GameManagerRef.UpdateHealthText(CurHealth.ToString());

            // Make immune
            IsImmune = true;
            ImmunityStartTime = Time.time;
        }
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "KillZone")
        {
            // Kill player
            ChangeHealth(-1000);
        }
        else if (col.gameObject.tag == "Enemy" && !IsImmune)
        {
            // Take damage
            ChangeHealth(-1);

            // Trigger damaged animation
            PlayerAnimator.SetTrigger("IsAttacked");

            // Make immune
            IsImmune = true;
            ImmunityStartTime = Time.time;
        }        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Potion")
        {
            if (CurHealth < MaxHealth)
            {
                // Restore health to full
                ChangeHealth(MaxHealth);

                // Hide potion
                col.gameObject.transform.parent.GetComponent<PotionSpawner>().HidePotion();
            }
        }
    }

    internal void ApplyHealthBoost()
    {
        MaxHealth += HealthBoost;

        if(CurHealth != MaxHealth)
        {
            CurHealth = MaxHealth;
        }        
    }
}
