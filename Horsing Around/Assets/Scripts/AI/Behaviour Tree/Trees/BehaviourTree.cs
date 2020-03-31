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
    internal GameManager GameManagerRef; // Ref to game manager
    protected Transform TargetRef; // Ref of object to target
    internal Transform AllyBase; // Position of the ally base
    internal Transform EnemyBase; // Position of the friendly base
    internal Transform HomeRef; // Position of home
    internal Transform BankRef; // Position of the bank
    internal Transform MineRef; // Position of the mine
    internal NavMeshAgent NavAgent; // Nav agent component ref
    internal Animator Anim; // Animator component ref
    internal float FleeOffset = 2f; // Amount to flee by

    // Health
    protected int Health; // Current health
    public int HealthMax = 10; // Max health

    // Stamina
    protected int Stamina; // Current stamina
    public int StaminaMax = 50; // Max stamina
    int StaminaMin = 49; // Min Stamina
    internal int StaminaRecoveryAmount = 1; // Amount to recover
    internal float StaminaRecoveryChance = 0.3f; // Percent chance to recover stamina that frame
    float MaxDistFromHome = 2f; // Max distance from home to rest up

    // Attacking
    public int AttackDamage = 1; // Damage to deal to enemies
    public float AttackDelay = 1f; // Delay in seconds
    public float AttackDelayRandOffsetMax = 1f; // Max random offset for attack delay
    internal float AttackDelayRandOffset = 0f; // Random offset for attack delay
    internal float LastAttackTime; // Time stamp of last attack    
    float AttackRadius = 1.5f;

    // Sight
    int MinEnemiesSpotted = 1;
    int MinPotionsClose = 1;
    int MinBlockadesClose = 1;
    int MinPlayerSighted = 1;

    // Tags need to match tags given to sight
    string PlayerTag = "Player";
    public string EnemyTag;
    public string AllyTag;
    public string PotionTag;
    public string BlockadeTag;

    // Miner
    float MaxDistFromBank = 2f; // Max distance from bank to deposit gold
    float MaxDistFromMine = 2f; // Max distance from mine to gather gold
    protected int CurrentGold = 0; // Current gold worker is carrying
    public int MaxGold = 50; // Max gold a worker can carry

    // Decision node structs
    Node_Decision.DecisionStruct DS_SeeEnemy = new Node_Decision.DecisionStruct("SeeEnemy", 0f, 0f); // succeed num = minimum enemies spotted
    Node_Decision.DecisionStruct DS_IsEnemyNear = new Node_Decision.DecisionStruct("IsEnemyNear", 0f, 0f); // succeed num = max distance away
    Node_Decision.DecisionStruct DS_CantAttack = new Node_Decision.DecisionStruct("CantAttack", 0f, 0f); // succeed num = last attack time
    Node_Decision.DecisionStruct DS_IsLowOnStamina = new Node_Decision.DecisionStruct("IsLowOnStamina", 0f, 0f); // succeed num = stamina min
    Node_Decision.DecisionStruct DS_IsAtHome = new Node_Decision.DecisionStruct("IsAtHome", 0f, 0f); // succeed num = max distance away
    Node_Decision.DecisionStruct DS_IsAtBank = new Node_Decision.DecisionStruct("IsAtBank", 0f, 0f); // succeed num = max distance away
    Node_Decision.DecisionStruct DS_IsAtMaxGold = new Node_Decision.DecisionStruct("IsAtMaxGold", 0f, 0f); // succeed num = max gold
    Node_Decision.DecisionStruct DS_IsAtMine = new Node_Decision.DecisionStruct("IsAtMine", 0f, 0f); // succeed num = max distance away
    Node_Decision.DecisionStruct DS_IsPotionClose = new Node_Decision.DecisionStruct("IsPotionClose", 0f, 0f); // succeed num = minimum potions close
    Node_Decision.DecisionStruct DS_IsBlockadeClose = new Node_Decision.DecisionStruct("IsBlockadeClose", 0f, 0f); // succeed num = minimum blockades close
    Node_Decision.DecisionStruct DS_AreEnemiesMorePowerful = new Node_Decision.DecisionStruct("AreEnemiesMorePowerful", 0f, 0f); // succeed num = ally count
    Node_Decision.DecisionStruct DS_IsPlayerSighted = new Node_Decision.DecisionStruct("IsPlayerSighted", 0f, 0f); // succeed num = min player count


    // Call this to move through the tree
    internal abstract void TraverseTree();

    // Child should define how to change the target
    internal abstract void ChangeTargetRef(Transform target);
    internal abstract void ChangeTargetRef(Vector3 newPos);

    // Child should define how it loses/gains resources
    internal abstract void ChangeHealth(int value);
    internal abstract void ChangeStamina(int value);
    internal abstract void ChangeGold(int value);

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

        // Set target to ally base
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, AllyBase);

        // Move to base action
        Node_Action moveToAllyBase = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Head to ally base children
        headToAllyBaseParent.NodeChildren.Add(setTarget); // Child = set target action node
        headToAllyBaseParent.NodeChildren.Add(moveToAllyBase); // Child = move to ally base action node

        return headToAllyBaseParent;
    }

    protected Node CheckCommandsBehaviour()
    {
        // Check commands parent
        Node_Composite checkCommandsParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);


        // Check command to get potion


        // Check command to go destroy blockade


        // Check command to retreat


        // Check command to attack player


        return checkCommandsParent;
    }

    /*/ Enemy of this gameobject branches /*/
    protected Node SimpleEnemyBehaviour()
    {
        // Find enemy parent
        Node_Composite simpleEnemyParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // See enemy?
        Node_Decision seeEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_SeeEnemy, this);

        // Set target to closest sighted action
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, 
                                                                                 Sight.CalculateClosestObject(EnemyTag, true).transform);

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

        // Set target to enemy base
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, EnemyBase);

        // Move to base action
        Node_Action moveToEnemyBase = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Head to enemy base children
        headToEnemyBaseParent.NodeChildren.Add(setTarget); // Child = set target action node
        headToEnemyBaseParent.NodeChildren.Add(moveToEnemyBase); // Child = move to enemy base action node

        return headToEnemyBaseParent;
    }

    /*/ Miner only branches /*/
    protected Node DepositGoldBehaviour()
    {
        // Deposit gold parent
        Node_Composite depositGoldParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Is at max gold?
        Node_Decision isAtMaxGold = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsAtMaxGold, this);

        // Is not low on stamina?
        Node_Decision isNotLowOnStamina = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherToPass, DS_IsLowOnStamina, this);

        // Reverse at bank
        Node_Decorator reverseAtBank = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.Reverse);

        // At bank sequence
        Node_Composite atBankSequence = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Is at bank?
        Node_Decision isAtBank = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsAtBank, this);

        // Set target to bank
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, BankRef);

        // Move to bank action
        Node_Action moveToBank = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Deposit gold action
        Node_Action depositGold = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.DepositGold, this);

        // Deposit gold parent children
        depositGoldParent.NodeChildren.Add(isAtMaxGold); // Child = is at max gold decision node
        depositGoldParent.NodeChildren.Add(isNotLowOnStamina); // Child = is not low on stamina decision node
        depositGoldParent.NodeChildren.Add(reverseAtBank); // Child = reverse at bank decorator node
        depositGoldParent.NodeChildren.Add(setTarget); // Child = set target action node
        depositGoldParent.NodeChildren.Add(moveToBank); // Child = move to bank action node

        // Reverse at bank children
        reverseAtBank.NodeChildren.Add(atBankSequence); // Child = at bank sequence node

        // At bank sequence children
        atBankSequence.NodeChildren.Add(isAtBank); // Child = is at bank decision node
        atBankSequence.NodeChildren.Add(depositGold); // Child = deposit gold action

        return depositGoldParent;
    }

    protected Node MineGoldBehaviour()
    {
        // Mine gold parent
        Node_Composite mineGoldParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Is not at max gold?
        Node_Decision isNotAtMaxGold = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerToPass, DS_IsAtMaxGold, this);

        // Reverse at mine
        Node_Decorator reverseAtMine = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.Reverse);

        // At mine sequence
        Node_Composite atMineSequence = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Is at mine?
        Node_Decision isAtMine = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsAtMine ,this);

        // Set target to mine ref
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, MineRef);

        // Move to mine action
        Node_Action moveToMine = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Mine gold action
        //Node_Action mineGold = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MineGold, this);

        // Mine gold parent children
        mineGoldParent.NodeChildren.Add(isNotAtMaxGold); // Child = is not at max gold decision node        
        mineGoldParent.NodeChildren.Add(reverseAtMine); // Child = reverse at mine decorator node
        mineGoldParent.NodeChildren.Add(setTarget); // Child = set target action node
        mineGoldParent.NodeChildren.Add(moveToMine); // Child = move to mine action node

        // Reverse at mine children
        reverseAtMine.NodeChildren.Add(atMineSequence); // Child = at mine sequence node

        // At mine sequence children
        atMineSequence.NodeChildren.Add(isAtMine); // Child = is at mine decision node
        //atMineSequence.NodeChildren.Add(mineGold); // Child = mine gold action node

        return mineGoldParent;
    }

    /*/ Commander branches /*/
    protected Node GiveCommandsBehaviour()
    {
        // Give commands parent
        Node_Composite giveCommandsParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check for potion
        Node_Decision isPotionClose = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsPotionClose, this);

        // Check allies health

        // Give command to lowest health ally to get potion


        // Check for blockade
        Node_Decision isBlockadeClose = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsBlockadeClose, this);

        // Check for no enemies
        Node_Decision seeNoEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerToPass, DS_SeeEnemy, this);

        // Find closest ally and set them as target

        // Give command to an ally to go destroy blockade
        //Node_Action giveCommandDestroy = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum. , this);


        // Check power of enemies vs power of allies
        Node_Decision areEnemiesMorePowerful = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherToPass, DS_AreEnemiesMorePowerful, this);

        // Give command to retreat
        //Node_Action giveCommandRetreat = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum. , this);


        // Check if player is nearby
        Node_Decision isPlayerSighted = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsPlayerSighted, this);

        // Find closest ally and set them as target

        // Give command to an ally to attack player
        //Node_Action giveCommandAttack = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum. , this);

        // Give commands children


        return giveCommandsParent;
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

        // Reverse at home
        Node_Decorator reverseAtHome = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.Reverse);

        // At home sequence
        Node_Composite atHomeSequence = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Is at home?
        Node_Decision isAtHome = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsAtHome, this);

        // Set target to home ref
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, HomeRef);

        // Move home
        Node_Action moveHome = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Rest up action
        Node_Action restUp = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.RestUp, this);

        // Rest up children
        restUpParent.NodeChildren.Add(isLowOnStamina); // Child = is low on stamina decision node
        restUpParent.NodeChildren.Add(reverseAtHome); // Child = reverse at home decorator node
        restUpParent.NodeChildren.Add(setTarget); // Child = set target action node
        restUpParent.NodeChildren.Add(moveHome); // Child = move home action node

        // Reverse at home children
        reverseAtHome.NodeChildren.Add(atHomeSequence); // Child  = at home sequence node

        // At home sequence children
        atHomeSequence.NodeChildren.Add(isAtHome); // Child = is at home decision node
        atHomeSequence.NodeChildren.Add(restUp); // Child = rest up action node
        

        return restUpParent;
    }

    // Supporting methods
    internal Node_Decision.DecisionStruct UpdateDecisionStruct(Node_Decision.DecisionStruct decisionConditions)
    {
        switch (decisionConditions.Name)
        {
            case "SeeEnemy":
                // Calc current enemies spotted
                int CurrentEnemiesSpotted = Sight.GetObjectsSpottedCount(EnemyTag);

                // Set condition numbers
                decisionConditions.SetConditions(CurrentEnemiesSpotted, MinEnemiesSpotted);
                break;

            case "IsEnemyNear":
                // Calc current enemies near
                float CurrentEnemyDist = Sight.CalculateClosestObjectDistance(EnemyTag, true);

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

            case "IsAtBank":
                // Calc distance to bank
                float distToBank = Vector3.Distance(transform.position, BankRef.position);

                // Set condition numbers
                decisionConditions.SetConditions(distToBank, MaxDistFromBank);
                break;

            case "IsAtMaxGold":
                // Set condition numbers
                decisionConditions.SetConditions(CurrentGold, MaxGold);
                break;

            case "IsAtMine":
                // Calc distance to mine
                float distToMine = Vector3.Distance(transform.position, BankRef.position);

                // Set condition numbers
                decisionConditions.SetConditions(distToMine, MaxDistFromMine);
                break;

            case "IsPotionClose":
                // Calc potions close
                int potionsClose = Sight.GetObjectsCloseCount(PotionTag);

                // Set condition numbers
                decisionConditions.SetConditions(potionsClose, MinPotionsClose);
                break;

            case "IsBlockadeClose":
                // Calc blockades close
                int blockadesClose = Sight.GetObjectsCloseCount(BlockadeTag);

                // Set condition numbers
                decisionConditions.SetConditions(blockadesClose, MinBlockadesClose);
                break;

            case "AreEnemiesMorePowerful":
                // Calc enemy power
                int enemyPower = Sight.GetObjectsCloseCount(EnemyTag);

                // Calc ally power
                int allyPower = Sight.GetObjectsCloseCount(AllyTag);

                // Set condition numbers
                decisionConditions.SetConditions(enemyPower, allyPower);
                break;

            case "IsPlayerSighted":
                // Calc player count
                int playerCount = Sight.GetObjectsSpottedCount(PlayerTag);

                // Set condition numbers
                decisionConditions.SetConditions(playerCount, MinPlayerSighted);
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

    internal int GetCurrentGold()
    {
        return CurrentGold;
    }
}
