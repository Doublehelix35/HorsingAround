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
    int LowHealth = 50; // This health or lower to get potion

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
    Transform EnemyRef; // Ref of current enemy
    Transform AllyRef; // Ref of current ally
    Transform PotionRef; // Ref of current potion
    Transform BlockadeRef; // Ref of blockade
    int MinEnemiesSpotted = 1;
    int MinPotionsClose = 1;
    int MinPotionsSighted = 1;
    float MinPotionDistance = 1f;
    int MinBlockadesClose = 1;
    int MinPlayerSighted = 1;
    int MinAllyClose = 1;

    // Commander
    int MinAlliesOnLowHealth = 1;
    int MinAllyHealth = 5;

    // Commands    
    internal Transform CommandersTarget; // Target given by the commander
    bool IsGetPotionActive = false;
    bool IsRetreatActive = false;
    bool IsAttackTargetActive = false;
    int MinCommands = 1;
    float MaxDistFromCommandersTarget = 1.5f;

    // Tags need to match tags given to sight
    string PlayerTag = "Player";
    public string EnemyTag;
    public string AllyTag;
    string PotionTag = "Potion";
    string BlockadeTag = "Blockade";

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
    Node_Decision.DecisionStruct DS_IsAllyLowHealth = new Node_Decision.DecisionStruct("IsAllyLowHealth", 0f, 0f); // succeed num = min ally at low health
    Node_Decision.DecisionStruct DS_IsAllyClose = new Node_Decision.DecisionStruct("IsAllyClose", 0f, 0f); // succeed num = min ally count
    Node_Decision.DecisionStruct DS_IsPotionSighted = new Node_Decision.DecisionStruct("IsPotionSighted", 0f, 0f); // succeed num = min potions seen
    Node_Decision.DecisionStruct DS_IsLowOnHealth = new Node_Decision.DecisionStruct("IsLowOnHealth", 0f, 0f); // succeed num = low health stat
    Node_Decision.DecisionStruct DS_IsPotionNear = new Node_Decision.DecisionStruct("IsPotionNear", 0f, 0f); // succeed num = max potion distance
    Node_Decision.DecisionStruct DS_CheckCommandPotion = new Node_Decision.DecisionStruct("CheckCommandPotion", 0f, 0f); // succeed num = get potion command active
    Node_Decision.DecisionStruct DS_CheckCommandRetreat = new Node_Decision.DecisionStruct("CheckCommandRetreat", 0f, 0f); // succeed num = retreat command active
    Node_Decision.DecisionStruct DS_CheckCommandAttackTarget = new Node_Decision.DecisionStruct("CheckCommandAttackTarget", 0f, 0f); // succeed num = attack target command active
    Node_Decision.DecisionStruct DS_IsCommandersTargetNear = new Node_Decision.DecisionStruct("IsCommandersTargetNear", 0f, 0f); // succeed num = max dist from target

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


        /*/ Potion command sequence /*/
        Node_Composite potionCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check command to get potion
        Node_Decision checkCommandPotion = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_CheckCommandPotion, this);

        // Potion not near?
        Node_Decision isPotionFar = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsCommandersTargetNear, this);


        /*/ Retreat command sequence /*/
        Node_Composite retreatCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check command to retreat
        Node_Decision checkCommandRetreat = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_CheckCommandRetreat, this);

        // Base not near?
        Node_Decision isBaseFar = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsCommandersTargetNear, this);


        /*/ Attack target command sequence /*/
        Node_Composite attackTargetCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check command to attack target
        Node_Decision checkCommandAttackTarget = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_CheckCommandAttackTarget, this);

        // Reverse target far
        Node_Decorator targetFarReversed = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.Reverse);

        // Target far sequence
        Node_Composite targetFarSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Target not near?
        Node_Decision isTargetFar = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsCommandersTargetNear, this);

        // Attack target
        Node_Action attackTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.AttackTarget, this);


        /*/ Reuse /*/
        // Set target to commanders target
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, CommandersTarget);

        // Move to target
        Node_Action moveToTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);


        // Check commands children
        checkCommandsParent.NodeChildren.Add(potionCommandSeq); // Child = potion command sequence node
        checkCommandsParent.NodeChildren.Add(retreatCommandSeq); // Child = retreat command sequence node
        checkCommandsParent.NodeChildren.Add(attackTargetCommandSeq); // Child = attack target command sequence node

        // Potion command seq children
        potionCommandSeq.NodeChildren.Add(checkCommandPotion); // Child = check command potion decision node
        potionCommandSeq.NodeChildren.Add(isPotionFar); // Child = is potion far decision node
        potionCommandSeq.NodeChildren.Add(setTarget); // Child = set target action node
        potionCommandSeq.NodeChildren.Add(moveToTarget); // Child = move to target action node

        // Retreat command seq children
        retreatCommandSeq.NodeChildren.Add(checkCommandRetreat); // Child = check command retreat decision node
        retreatCommandSeq.NodeChildren.Add(isBaseFar); // Child = is base far decision node
        retreatCommandSeq.NodeChildren.Add(setTarget); // Child = set target action node
        retreatCommandSeq.NodeChildren.Add(moveToTarget); // Child = move to target action node

        // Attack target command seq children
        attackTargetCommandSeq.NodeChildren.Add(checkCommandAttackTarget); // Child = check command blockade decision node
        attackTargetCommandSeq.NodeChildren.Add(setTarget); // Child = set target action node
        attackTargetCommandSeq.NodeChildren.Add(targetFarReversed); // Child = target far reversed decorator node
        attackTargetCommandSeq.NodeChildren.Add(attackTarget); // Child = attack target action node        

        // Target far reversed children
        targetFarReversed.NodeChildren.Add(targetFarSeq); // Child = target far sequence node

        // Target far seq children
        targetFarSeq.NodeChildren.Add(isTargetFar); // Child = is target far decision node
        targetFarSeq.NodeChildren.Add(moveToTarget); // Child = move to target action node


        return checkCommandsParent;
    }

    /*/ Enemy of this gameobject branches /*/
    protected Node SimpleEnemyBehaviour()
    {
        // Find enemy parent
        Node_Composite simpleEnemyParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // See enemy?
        Node_Decision seeEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_SeeEnemy, this);

        // Set target to enemy
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, EnemyRef);

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

        // Isn't at mine?
        Node_Decision isntAtMine = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherToPass, DS_IsAtMine ,this);

        // Set target to mine ref
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, MineRef);

        // Move to mine action
        Node_Action moveToMine = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);

        // Mine gold action
        //Node_Action mineGold = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MineGold, this);

        // Mine gold parent children
        mineGoldParent.NodeChildren.Add(isNotAtMaxGold); // Child = is not at max gold decision node        
        mineGoldParent.NodeChildren.Add(isntAtMine); // Child = reverse at mine decorator node
        mineGoldParent.NodeChildren.Add(setTarget); // Child = set target action node
        mineGoldParent.NodeChildren.Add(moveToMine); // Child = move to mine action node


        return mineGoldParent;
    }

    /*/ Commander branches /*/
    protected Node GiveCommandsBehaviour()
    {
        // Give commands parent
        Node_Composite giveCommandsParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check for ally
        Node_Decision isAllyClose = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsAllyClose, this);


        /*/ Potion command sequence /*/
        Node_Composite potionCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check for potion
        Node_Decision isPotionClose = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsPotionClose, this);

        // Check allies health
        Node_Decision isAllyLowHealth = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsAllyLowHealth, this);

        // Set commanders target to potion
        Node_Action setCommandersTargetPotion = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetCommandersTarget, this, PotionRef);

        // Give command to ally
        Node_Action giveCommandPotion = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.GiveCommand, this, AllyRef, BT_Commander.Commands.GetPotion);


        /*/ Blockade command sequence /*/
        Node_Composite blockadeCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check for blockade
        Node_Decision isBlockadeClose = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsBlockadeClose, this);

        // Check for no enemies
        Node_Decision seeNoEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerToPass, DS_SeeEnemy, this);

        // Set commanders target to blockade
        Node_Action setCommandersTargetBlockade = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetCommandersTarget, this, BlockadeRef);

        // Give command to an ally to go destroy blockade
        Node_Action giveCommandBlockade = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.GiveCommand, this, AllyRef, BT_Commander.Commands.AttackTarget);


        /*/ Retreat command sequence /*/
        Node_Composite retreatCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check power of enemies vs power of allies
        Node_Decision areEnemiesMorePowerful = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherToPass, DS_AreEnemiesMorePowerful, this);

        // Set commanders target to base
        Node_Action setCommandersTargetRetreat = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetCommandersTarget, this, AllyBase);

        // Give command to retreat
        Node_Action giveCommandRetreat = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.GiveCommand, this, AllyRef, BT_Commander.Commands.Retreat);


        /*/ Attack player command sequence /*/
        Node_Composite attackPlayerCommandSeq = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Check if player is nearby
        Node_Decision isPlayerSighted = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsPlayerSighted, this);

        // Set commanders target to player
        Node_Action setCommandersTargetPlayer = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetCommandersTarget, this, EnemyRef);

        // Give command to an ally to attack player
        Node_Action giveCommandAttack = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.GiveCommand, this, AllyRef , BT_Commander.Commands.AttackTarget);


        // Give commands children
        giveCommandsParent.NodeChildren.Add(isAllyClose); // Child = is ally close decision node
        giveCommandsParent.NodeChildren.Add(potionCommandSeq); // Child = potion command sequence node
        giveCommandsParent.NodeChildren.Add(blockadeCommandSeq); // Child = blockade command sequence node
        giveCommandsParent.NodeChildren.Add(retreatCommandSeq); // Child = retreat command sequence node
        giveCommandsParent.NodeChildren.Add(attackPlayerCommandSeq); // Child = attack player command sequence node

        // Potion command seq children
        potionCommandSeq.NodeChildren.Add(isPotionClose); // Child = is potion close decision node
        potionCommandSeq.NodeChildren.Add(isAllyLowHealth); // Child = is ally low health decision node
        potionCommandSeq.NodeChildren.Add(setCommandersTargetPotion); // Child = set commanders target potion action node
        potionCommandSeq.NodeChildren.Add(giveCommandPotion); // Child = give command potion action node

        // Blockade command seq children
        blockadeCommandSeq.NodeChildren.Add(isBlockadeClose); // Child = is blockade close decision node
        blockadeCommandSeq.NodeChildren.Add(seeNoEnemy); // Child = see no enemy decision node
        blockadeCommandSeq.NodeChildren.Add(setCommandersTargetBlockade); // Child = set commanders target blockade action node
        blockadeCommandSeq.NodeChildren.Add(giveCommandBlockade); // Child = give command blockade action node

        // Retreat command seq children
        retreatCommandSeq.NodeChildren.Add(areEnemiesMorePowerful); // Child = are enemies more powerful decision node
        retreatCommandSeq.NodeChildren.Add(setCommandersTargetRetreat); // Child = set commanders target retreat action node
        retreatCommandSeq.NodeChildren.Add(giveCommandRetreat); // Child = give command retreat action node

        // Attack player command seq children
        attackPlayerCommandSeq.NodeChildren.Add(isPlayerSighted); // Child = is player sighted decision node
        attackPlayerCommandSeq.NodeChildren.Add(setCommandersTargetPlayer); // Child = set commanders target player action node
        attackPlayerCommandSeq.NodeChildren.Add(giveCommandAttack); // Child = give command attack action node

        return giveCommandsParent;
    }

    /*/ Misc branches /*/
    protected Node HealthPotionBehaviour()
    {
        // Health potion parent
        Node_Composite healthPotionParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // Low health?
        Node_Decision isLowOnHealth = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerOrEqualToPass, DS_IsLowOnHealth, this);

        // See potion?
        Node_Decision isPotionSighted = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsPotionSighted, this);

        // Potion not near?
        Node_Decision isPotionFar = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherOrEqualToPass, DS_IsPotionNear, this);

        // Set target to potion
        Node_Action setTarget = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.SetTarget, this, PotionRef);

        // Move to potion
        Node_Action moveToPotion = gameObject.AddComponent<Node_Action>().SetUpNode(Node_Action.ActionTypeEnum.MoveToTarget, this);


        // Health potion children
        healthPotionParent.NodeChildren.Add(isLowOnHealth); // Child = is low on health decision node
        healthPotionParent.NodeChildren.Add(isPotionSighted); // Child = is potion sighted decision node
        healthPotionParent.NodeChildren.Add(isPotionFar); // Child = is potion far decision node
        healthPotionParent.NodeChildren.Add(setTarget); // Child = set target action node
        healthPotionParent.NodeChildren.Add(moveToPotion); // Child = move to target action node

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

                // If enemy spotted update enemy ref
                if(CurrentEnemiesSpotted >= MinEnemiesSpotted)
                {
                    EnemyRef = Sight.CalculateClosestObject(EnemyTag, true).transform;
                }

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

                if (potionsClose >= MinPotionsClose)
                {
                    PotionRef = Sight.CalculateClosestObject(PotionTag, false).transform;
                }

                // Set condition numbers
                decisionConditions.SetConditions(potionsClose, MinPotionsClose);
                break;

            case "IsBlockadeClose":
                // Calc blockades close
                int blockadesClose = Sight.GetObjectsCloseCount(BlockadeTag);

                // Set blockade ref
                if(blockadesClose >= MinBlockadesClose)
                {
                    BlockadeRef = Sight.CalculateClosestObject(BlockadeTag, false).transform;
                }

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

                // Set enemy ref
                if(playerCount >= MinPlayerSighted)
                {
                    EnemyRef = Sight.CalculateClosestObject(PlayerTag, true).transform;
                }

                // Set condition numbers
                decisionConditions.SetConditions(playerCount, MinPlayerSighted);
                break;

            case "IsAllyLowHealth":
                // Calc allies on low health
                int alliesOnLowHealth = 0;

                List<GameObject> tempList = Sight.GetObjectSpottedList(AllyTag);

                for(int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i].GetComponent<BehaviourTree>().Health < MinAllyHealth)
                    {
                        alliesOnLowHealth++;

                        // Exit loop if further loops are excessive
                        if(alliesOnLowHealth >= MinAlliesOnLowHealth)
                        {
                            // Set ally ref
                            AllyRef = tempList[i].transform;
                            break;
                        }                        
                    }
                }

                // Set condition numbers
                decisionConditions.SetConditions(alliesOnLowHealth, MinAlliesOnLowHealth);
                break;

            case "IsAllyClose":
                // Calc allies close
                int allyCount = Sight.GetObjectsCloseCount(AllyTag);

                // Set ally ref
                if(allyCount >= MinAllyClose)
                {
                    AllyRef = Sight.CalculateClosestObject(AllyTag, false).transform;
                }

                // Set condition numbers
                decisionConditions.SetConditions(allyCount, MinAllyClose);
                break;

            case "IsPotionSighted":
                // Calc potions sighted
                int potionsSighted = Sight.GetObjectsSpottedCount(PotionTag);

                // If potion spotted update potion ref
                if (potionsSighted >= MinPotionsSighted)
                {
                    PotionRef = Sight.CalculateClosestObject(PotionTag, true).transform;
                }

                // Set condition numbers
                decisionConditions.SetConditions(potionsSighted, MinPotionsSighted);
                break;

            case "IsLowOnHealth":
                // Set condition numbers
                decisionConditions.SetConditions(Health, LowHealth);
                break;

            case "IsPotionNear":
                // Calc potion dist
                float potionDist = Sight.CalculateClosestObjectDistance(PotionTag, true);

                // Set condition numbers
                decisionConditions.SetConditions(potionDist, MinPotionDistance);
                break;

            case "CheckCommandPotion":
                // Set potion command status
                int potionCommandStatus = IsGetPotionActive ? 1 : 0;

                // Set condition numbers
                decisionConditions.SetConditions(potionCommandStatus, MinCommands);
                break;

            case "CheckCommandRetreat":
                // Set retreat command status
                int retreatCommandStatus = IsRetreatActive ? 1 : 0;

                // Set condition numbers
                decisionConditions.SetConditions(retreatCommandStatus, MinCommands);
                break;

            case "CheckCommandAttackTarget":
                // Set attack target command status
                int attackTargetCommandStatus = IsAttackTargetActive ? 1 : 0;

                // Set condition numbers
                decisionConditions.SetConditions(attackTargetCommandStatus, MinCommands);
                break;

            case "IsCommandersTargetNear":
                // Calc dist
                float distToTarget = Vector3.Distance(transform.position, CommandersTarget.position);

                // Set condition numbers
                decisionConditions.SetConditions(distToTarget, MaxDistFromCommandersTarget);
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

    internal void ReceiveCommand(BT_Commander.Commands command)
    {

        switch (command)
        {
            case BT_Commander.Commands.AttackTarget:                
                IsAttackTargetActive = true;
                break;

            case BT_Commander.Commands.GetPotion:
                IsGetPotionActive = true;
                break;

            case BT_Commander.Commands.Retreat:
                IsRetreatActive = true;
                break;

            default:
                Debug.Log("Command not recognized");
                break;
        }
    }
}
