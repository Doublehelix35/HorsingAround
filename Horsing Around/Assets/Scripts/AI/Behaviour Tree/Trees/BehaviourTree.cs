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
    protected Transform TargetRef; // Ref of object to target
    internal Transform AllyBase; // Position of the ally base
    internal Transform EnemyBase; // Position of the friendly base
    internal Transform HomeRef; // Position of home
    internal NavMeshAgent NavAgent; // Nav agent component ref
    internal Animator Anim; // Animator component ref
    internal float FleeOffset = 2f; // Amount to flee by

    // Health
    protected int Health; // Current health
    public int HealthMax = 10; // Max health

    // Stamina
    protected int Stamina; // Current stamina
    public int StaminaMax = 20; // Max stamina
    int StaminaMin = 0; // Min Stamina
    float MaxDistFromHome = 2f; // Max distance from home to rest up

    // Attacking
    public int AttackDamage = 1; // Damage to deal to enemies
    public float AttackDelay = 1f; // Delay in seconds
    public float AttackDelayRandOffsetMax = 1f; // Max random offset for attack delay
    internal float AttackDelayRandOffset = 0f; // Random offset for attack delay
    internal float LastAttackTime; // Time stamp of last attack    

    // See enemy
    int MinEnemiesSpotted = 1;
    int CurrentEnemiesSpotted;

    // Enemies close
    float AttackRadius = 1.5f;
    float CurrentEnemyDist;

    // Decision node structs
    Node_Decision.DecisionStruct DS_SeeEnemy = new Node_Decision.DecisionStruct("SeeEnemy", 0f, 0f); // succeed num = minimum enemies spotted
    Node_Decision.DecisionStruct DS_IsEnemyNear = new Node_Decision.DecisionStruct("IsEnemyNear", 0f, 0f); // succeed num = max distance away
    Node_Decision.DecisionStruct DS_CantAttack = new Node_Decision.DecisionStruct("CantAttack", 0f, 0f); // succeed num = last attack time
    Node_Decision.DecisionStruct DS_IsLowOnStamina = new Node_Decision.DecisionStruct("IsLowOnStamina", 0f, 0f); // succeed num = stamina min
    Node_Decision.DecisionStruct DS_IsAtHome = new Node_Decision.DecisionStruct("IsAtHome", 0f, 0f); // succeed num = max distance away

    // Call this to move through the tree
    internal abstract void TraverseTree();

    // Child should define how to change the target
    internal abstract void ChangeTargetRef(Transform target);
    internal abstract void ChangeTargetRef(Vector3 newPos);

    // Child should define how it takes damage/heals
    internal abstract void ChangeHealth(int value);

    // Child should define how it loses/gains stamina
    internal abstract void ChangeStamina(int value);

    /*/ Ally of this gameobject branches /*/
    protected Node SimpleAllyBehaviour()
    {
        // Ally parent
        Node_Composite simpleAllyParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Stay close to ally

        return simpleAllyParent;
    }

    protected Node HeadToAllyBaseBehaviour()
    {
        // Head to ally base parent
        Node_Composite headToAllyBaseParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Set target action
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, AllyBase);

        // Move to base action
        Node_Action moveToAllyBase = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Head to ally base children
        headToAllyBaseParent.NodeChildren.Add(setTarget); // Child = set target action node
        headToAllyBaseParent.NodeChildren.Add(moveToAllyBase); // Child = move to ally base action node

        return headToAllyBaseParent;
    }


    /*/ Enemy of this gameobject branches /*/
    protected Node SimpleEnemyBehaviour()
    {
        // Find enemy parent
        Node_Composite simpleEnemyParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // See enemy?
        Node_Decision seeEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_SeeEnemy, this);

        // Set target to closest sighted action
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, Sight.CalculateClosestObject(true).transform);

        // Reverse enemy near sequence
        Node_Decorator enemyNearReversed = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.Reverse);

        // Enemy near sequence        
        Node_Composite enemyNearSequence = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check distance to enemy
        Node_Decision isEnemyNear = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsEnemyNear, this);

        // Attack selector
        Node_Composite attackSelector = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Check cant attack
        Node_Decision cantAttack = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_CantAttack , this);

        // Attack action
        Node_Action attackEnemy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.AttackTarget, this);

        // Move to enemy action
        Node_Action moveToEnemy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);        

        // Enemy parent children
        simpleEnemyParent.NodeChildren.Add(seeEnemy); // Child = see enemy decision node
        simpleEnemyParent.NodeChildren.Add(setTarget); // Child = set target action node
        simpleEnemyParent.NodeChildren.Add(enemyNearReversed); // Child = enemy near reversed decorator node
        simpleEnemyParent.NodeChildren.Add(moveToEnemy); // Child = attack enemy action node

        // Enemy near reversed children
        enemyNearReversed.NodeChildren.Add(enemyNearSequence); // Child = enemy near sequence node

        // Enemy near sequence children
        enemyNearSequence.NodeChildren.Add(isEnemyNear); // Child = is enemy near decorator node
        enemyNearSequence.NodeChildren.Add(attackSelector); // Child = attack selector node

        // Attack selector children
        attackSelector.NodeChildren.Add(cantAttack); // Child = cant attack decision node
        attackSelector.NodeChildren.Add(attackEnemy); // Child = move to enemy action node

        return simpleEnemyParent;
    }

    protected Node HeadToEnemyBaseBehaviour()
    {
        // Head to enemy base parent
        Node_Composite headToEnemyBaseParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Set target action
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, EnemyBase);

        // Move to base action
        Node_Action moveToEnemyBase = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Head to enemy base children
        headToEnemyBaseParent.NodeChildren.Add(setTarget); // Child = set target action node
        headToEnemyBaseParent.NodeChildren.Add(moveToEnemyBase); // Child = move to enemy base action node

        return headToEnemyBaseParent;
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

    protected Node RestUpBehaviour()
    {
        // Rest up parent
        Node_Composite restUpParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Low on stamina?
        Node_Decision isLowOnStamina = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsLowOnStamina, this);

        // Is at home?
        Node_Decision isAtHome = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsAtHome, this);

        // Set target
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, HomeRef);

        // Move home
        Node_Action moveHome = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Rest up action
        Node_Action restUp = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.RestUp, this);

        // Rest up children

        return restUpParent;
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

            case "CantAttack":
                // Set condition numbers
                decisionConditions.SetConditions(Time.time, LastAttackTime + AttackDelay + AttackDelayRandOffset);
                break;

            case "IsLowOnStamina":
                // Set condition numbers
                decisionConditions.SetConditions(Stamina, StaminaMin);
                break;

            case "IsAtHome":
                // Calc distance to home
                float distToHome = Vector3.Distance(transform.position, HomeRef.position);

                // Set condition numbers
                decisionConditions.SetConditions(distToHome, MaxDistFromHome);
                break;

            default:
                Debug.Log("Decision struct name not found!");
                break;
        }

        // Return the updated conditions
        return decisionConditions;
    }    

    internal Transform GetTargetRef()
    {
        return TargetRef;
    }
}
