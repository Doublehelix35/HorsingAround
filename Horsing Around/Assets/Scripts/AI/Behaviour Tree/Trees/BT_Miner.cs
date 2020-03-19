using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BT_Miner : BehaviourTree
{
    // Miner tree
    Node_Decorator StartNode; // First node

    internal Transform BankRef; // Position of the bank

    void Awake()
    {
        // Init variables
        TargetRef = GameObject.FindGameObjectWithTag("Player").transform;
        AllyBase = GameObject.FindGameObjectWithTag("PlayerBase").transform;
        EnemyBase = GameObject.FindGameObjectWithTag("EnemyBase").transform;
        NavAgent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        Health = HealthMax;
        Stamina = StaminaMax;

        // Init nodes
        StartNode = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.RepeatTilFail);
        Node_Composite selectBehaviourNode = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Set up children
        StartNode.NodeChildren.Add(selectBehaviourNode); // Child = Select behaviour node
        selectBehaviourNode.NodeChildren.Add(RestUpBehaviour()); // Child = Rest up behaviour node
        selectBehaviourNode.NodeChildren.Add(DepositGoldBehaviour()); // Child = Deposit gold behaviour node
        selectBehaviourNode.NodeChildren.Add(MineGoldBehaviour()); // Child = Mine gold behaviour node
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
        Stamina = Mathf.Clamp(value, 0, StaminaMax);
    }

    /*/ Miner only branches /*/
    protected Node MineGoldBehaviour()
    {
        // Mine gold parent
        Node_Composite mineGoldParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // At mine?

        // Move to mine action

        // Mine gold action

        return mineGoldParent;
    }

    protected Node DepositGoldBehaviour()
    {
        // Deposit gold parent
        Node_Composite depositGoldParent = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Sequence);

        // At bank?

        // Move to bank action

        // Deposit gold action

        return depositGoldParent;
    }
}
