﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class BehaviourTree : MonoBehaviour
{
    // Variables for branches
    protected GameObject TargetRef; // Ref of object to target
    protected Vector3 FriendlyBase; // Position of the friendly base
    protected Vector3 EnemyBase; // Position of the friendly base
    internal NavMeshAgent NavAgent; // Nav agent component ref
    internal Animator Anim; // Animator component ref
    protected float Health; // Current health
    public float HealthMax = 10; // Max health
    internal float FleeOffset = 2f; // Amount to flee by

    // See enemy
    int MinEnemiesSpotted = 1;
    int CurrentEnemiesSpotted = 1;

    // Enemies close
    float MinDistToEnemy = 1;
    float CurrentEnemyDist = 2;

    // Decision node structs
    Node_Decision.DecisionStruct DS_SeeEnemy = new Node_Decision.DecisionStruct("SeeEnemy", 0, 0); // succeed num = min 1 enemy
    Node_Decision.DecisionStruct DS_IsEnemyNear = new Node_Decision.DecisionStruct("IsEnemyNear", 0, 0); // succeed num = max 1 unit away


    // Call this to move through the tree
    internal abstract void TraverseTree();

    // Child should define how to get the target
    internal abstract void ChangeTargetRef(GameObject target);

    // Child should define how it takes damage/heals
    internal abstract void ChangeHealth(int value);

    /*/ Ally of this gameobject branches /*/
    protected Node SimpleAllyBehaviour()
    {
        // Ally parent
        Node_Composite simpleAllyParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Stay close to ally

        return simpleAllyParent;
    }


    /*/ Enemy of this gameobject branches /*/
    protected Node SimpleEnemyBehaviour()
    {
        // Find enemy parent
        Node_Composite simpleEnemyParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // See enemy?
        UpdateDecisionStruct(DS_SeeEnemy);
        Node_Decision seeEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_SeeEnemy, this);

        // Enemy near selector        
        Node_Composite enemyNearSelector = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Check distance to enemy
        UpdateDecisionStruct(DS_IsEnemyNear);
        Node_Decision isEnemyNear = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsEnemyNear, this);

        // Move to enemy action
        Node_Action moveToEnemy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this, TargetRef);

        // Attack action
        Node_Action attackEnemy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.AttackTarget, this, TargetRef);

        //Set up children
        // Enemy parent children
        simpleEnemyParent.NodeChildren.Add(seeEnemy); // Child = see enemy decision node
        simpleEnemyParent.NodeChildren.Add(enemyNearSelector); // Child = enemy near selector node
        simpleEnemyParent.NodeChildren.Add(attackEnemy); // Child = attack enemy action node

        // Enemy near selector children
        enemyNearSelector.NodeChildren.Add(isEnemyNear); // Child = is enemy near decorator node
        enemyNearSelector.NodeChildren.Add(moveToEnemy); // Child = move to enemy action node

        return simpleEnemyParent;
    }

    protected Node HeadToEnemyBaseBehaviour()
    {
        // Head to enemy base parent
        Node_Composite headToEnemyBaseParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Move to base action

        return headToEnemyBaseParent;
    }

    /*/ Miner branches /*/
    protected Node MineGoldBehaviour()
    {
        // Mine gold parent
        Node_Composite mineGoldParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // At mine?

        // Move to mine action

        // Mine gold action

        return mineGoldParent;
    }


    /*/ Misc branches /*/
    protected Node HealthPotionBehaviour()
    {
        // Health potion parent
        Node_Composite healthPotionParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // See potion?

        // Get potion

        // Use potion action

        return healthPotionParent;
    }    

    protected Node BlockadeBehaviour()
    {
        // Blockade parent
        Node_Composite blockadeParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Blockade close?

        // Random chance

        // Destroy blockade

        return blockadeParent;
    }

    // Supporting methods
    internal Node_Decision.DecisionStruct UpdateDecisionStruct(Node_Decision.DecisionStruct decisionConditions)
    {
        switch (decisionConditions.Name)
        {
            case "SeeEnemy":
                // Calc current enemies spotted
                //CurrentEnemiesSpotted = 1;

                // Set condition numbers
                decisionConditions.SetConditions(CurrentEnemiesSpotted, MinEnemiesSpotted);
                break;

            case "IsEnemyNear":
                // Calc current enemies near
                //CurrentEnemyDist = 1;

                // Set condition numbers
                decisionConditions.SetConditions(CurrentEnemyDist, MinDistToEnemy);
                break;

            default:
                break;
        }

        // Return the updated conditions
        return decisionConditions;
    }    
}