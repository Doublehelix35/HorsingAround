using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class BehaviourTree : MonoBehaviour
{
    // Variables for branches
    public Sense_Sight Sight; // Sight ref
    protected GameObject TargetRef; // Ref of object to target
    protected Vector3 FriendlyBase; // Position of the friendly base
    protected Vector3 EnemyBase; // Position of the friendly base
    internal NavMeshAgent NavAgent; // Nav agent component ref
    internal Animator Anim; // Animator component ref
    protected float Health; // Current health
    public float HealthMax = 10; // Max health
    public int AttackDamage = 1; // Damage to deal to enemies
    internal float FleeOffset = 2f; // Amount to flee by

    // See enemy
    int MinEnemiesSpotted = 1;
    int CurrentEnemiesSpotted;

    // Enemies close
    float AttackRadius = 1.5f;
    float CurrentEnemyDist;

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
        Node_Decision seeEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_SeeEnemy, this);

        // Set target action
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTargetToSighted, this);

        // Reverse enemy near sequence
        Node_Decorator enemyNearReversed = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.Reverse);

        // Enemy near sequence        
        Node_Composite enemyNearSequence = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check distance to enemy
        Node_Decision isEnemyNear = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsEnemyNear, this);

        // Attack action
        Node_Action attackEnemy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.AttackTarget, this);

        // Move to enemy action
        Node_Action moveToEnemy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);        

        //Set up children
        // Enemy parent children
        simpleEnemyParent.NodeChildren.Add(seeEnemy); // Child = see enemy decision node
        simpleEnemyParent.NodeChildren.Add(setTarget); // Child = set target action node
        simpleEnemyParent.NodeChildren.Add(enemyNearReversed); // Child = enemy near reversed decorator node
        simpleEnemyParent.NodeChildren.Add(moveToEnemy); // Child = attack enemy action node

        // Enemy near reversed children
        enemyNearReversed.NodeChildren.Add(enemyNearSequence); // Child = enemy near sequence node

        // Enemy near selector children
        enemyNearSequence.NodeChildren.Add(isEnemyNear); // Child = is enemy near decorator node
        enemyNearSequence.NodeChildren.Add(attackEnemy); // Child = move to enemy action node

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
                CurrentEnemiesSpotted = Sight.ObjectsSpottedCount;

                // Set condition numbers
                decisionConditions.SetConditions(CurrentEnemiesSpotted, MinEnemiesSpotted);
                break;

            case "IsEnemyNear":
                // Calc current enemies near
                CurrentEnemyDist = Sight.CalculateClosestObjectDistance(true);

                // Set condition numbers
                decisionConditions.SetConditions(CurrentEnemyDist, AttackRadius);
                break;

            default:
                break;
        }

        // Return the updated conditions
        return decisionConditions;
    }    

    internal GameObject GetTargetRef()
    {
        return TargetRef;
    }
}
