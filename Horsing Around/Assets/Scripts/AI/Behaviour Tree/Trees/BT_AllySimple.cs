﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BT_AllySimple : BehaviourTree
{
    // Simple ally tree
    Node_Decorator StartNode; // First node

    void Awake()
    {
        // Init variables
        GameManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        AllyBase = GameManagerRef.GiveUnitPatrolSpot();
        EnemyBase = GameObject.FindGameObjectWithTag("EnemyBase").transform;
        NavAgent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Health = HealthMax;
        Stamina = StaminaMax;
        CurrentGold = StartingGold;

        // Init nodes
        StartNode = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.RepeatTilFail);
        Node_Composite selectBehaviourNode = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Set up children
        StartNode.NodeChildren.Add(selectBehaviourNode); // Child = Select behaviour node
        selectBehaviourNode.NodeChildren.Add(HealthPotionBehaviour()); // Child = Health potion behaviour node
        selectBehaviourNode.NodeChildren.Add(SimpleEnemyBehaviour()); // Child = Simple enemy behaviour node
        //selectBehaviourNode.NodeChildren.Add(SimpleAllyBehaviour()); // Child = Simple ally behaviour node
        //selectBehaviourNode.NodeChildren.Add(BlockadeBehaviour()); // Child = Blockade behaviour node
        selectBehaviourNode.NodeChildren.Add(HeadToAllyBaseBehaviour()); // Child = Head to ally base behaviour node
    }

    void FixedUpdate()
    {
        // Only traverse tree once a tick
        if (Time.time >= LastTickTime + TickDelay)
        {
            TraverseTree();
            LastTickTime = Time.time;
        }

        // Calc speed (between 0 and 1) and give it to the animator
        float speed = NavAgent.velocity.magnitude / NavAgent.speed;
        Anim.SetFloat("Speed", speed);
    }

    override internal void TraverseTree()
    {
        // Call current node process node()
        Node.NodeStatus status = StartNode.ProcessNode();
        //Debug.Log("Status = " + status);
    }

    // Set the target ref to a new target
    internal override void ChangeTargetRef(Transform target)
    {
        TargetRef = target;
    }

    // Changes the position of the current target
    internal override void ChangeTargetRef(Vector3 newPos)
    {
        TargetRef.position = newPos;
    }

    internal override void ChangeHealth(int value)
    {
        Health += value;

        // Clamp between 0 and max health
        Health = Mathf.Clamp(Health, 0, HealthMax);

        if (Health <= 0) // Killed
        {
            // Play death anim
            Anim.SetBool("IsDead", true);

            // Delay

            // Drop gold
            GameObject coin = Instantiate(GoldCoinPrefab, transform.position, Quaternion.identity);
            coin.GetComponent<GoldCoin>().GoldAmount = CurrentGold;

            // Kill self
            Destroy(gameObject);
        }
        else if (value < 0) // Damaged
        {
            // Play damaged anim
            Anim.SetTrigger("IsAttacked");
        }
        else // Healed
        {
            // Play healed anim or particle effect
        }
    }

    internal override void ChangeStamina(int value)
    {
        // Add value to stamina
        Stamina += value;

        // Clamp between 0 and max stamina
        Stamina = Mathf.Clamp(Stamina, 0, StaminaMax);
    }

    internal override void ChangeGold(int value)
    {
        // Add value to gold
        CurrentGold += value;

        // Clamp between 0 and max gold
        CurrentGold = Mathf.Clamp(CurrentGold, 0, MaxGold);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "KillZone")
        {
            // Kill self
            ChangeHealth(-1000);
        }

        if (col.gameObject.tag == "Potion")
        {
            if(Health < HealthMax)
            {
                // Restore health to full
                ChangeHealth(HealthMax);

                // Hide potion
                col.gameObject.transform.parent.GetComponent<PotionSpawner>().HidePotion();
            }            
        }
    }
}
