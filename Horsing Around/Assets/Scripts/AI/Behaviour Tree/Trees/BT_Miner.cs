using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BT_Miner : BehaviourTree
{
    // Miner tree
    Node_Decorator StartNode; // First node
    public Renderer[] MinerRenderers;
    bool IsVisible = true;
    int MiningStaminaCost = 1;

    void Awake()
    {
        // Init variables
        GameManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        TargetRef = GameObject.FindGameObjectWithTag("Player").transform;
        AllyBase = GameObject.FindGameObjectWithTag("PlayerBase").transform;
        MineRef = GameManagerRef.PlaceWorkerInMine(this);
        HomeRef = GameManagerRef.PlaceUnitInHouse();
        BankRef = GameObject.FindGameObjectWithTag("Bank").transform;        
        NavAgent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Health = HealthMax;
        Stamina = StaminaMax;

        // Init nodes
        StartNode = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.RepeatTilFail);
        Node_Composite selectBehaviourNode = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Set up children
        StartNode.NodeChildren.Add(selectBehaviourNode); // Child = Select behaviour node
        selectBehaviourNode.NodeChildren.Add(MineGoldBehaviour()); // Child = Mine gold behaviour node        
        selectBehaviourNode.NodeChildren.Add(DepositGoldBehaviour()); // Child = Deposit gold behaviour node  
        selectBehaviourNode.NodeChildren.Add(RestUpBehaviour()); // Child = Rest up behaviour node      
        selectBehaviourNode.NodeChildren.Add(HeadToAllyBaseBehaviour()); // Child = Head to ally base behaviour node        
    }

    void FixedUpdate()
    {
        TraverseTree();

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
        int tempGold = CurrentGold;

        // Add value to gold
        CurrentGold += value;

        // Clamp between 0 and max gold
        CurrentGold = Mathf.Clamp(CurrentGold, 0, MaxGold);

        // Check if gold gained
        if(CurrentGold > tempGold)
        {
            ChangeStamina(-MiningStaminaCost);
        }
    }

    internal void ToggleVisibility()
    {
        // Toggle is visible
        IsVisible = !IsVisible;

        for(int i = 0; i < MinerRenderers.Length; i++)
        {
            if(MinerRenderers[i] != null)
            {
                MinerRenderers[i].enabled = IsVisible;
            }
        }
    }

     internal void SetVisibility(bool isVisible)
    {
        // Set is visible
        IsVisible = isVisible;

        for (int i = 0; i < MinerRenderers.Length; i++)
        {
            if (MinerRenderers[i] != null)
            {
                MinerRenderers[i].enabled = IsVisible;
            }
        }
    }
}
