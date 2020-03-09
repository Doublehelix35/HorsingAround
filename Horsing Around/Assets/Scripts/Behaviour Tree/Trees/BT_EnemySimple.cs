using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_EnemySimple : BehaviourTree
{
    // Simple enemy tree
    Node_Decorator StartNode; // First node

    void Awake()
    {
        // Init variables
        // TargetRef = new GameObject();
        // FriendlyBase = base.transform.position;
        // EnemyBase = base.transform.position;

        // Init nodes
        StartNode = gameObject.AddComponent<Node_Decorator>().SetUpNode(Node_Decorator.DecoratorNodeType.RepeatTilFail);
        Node_Composite selectBehaviourNode = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Set up children
        StartNode.NodeChildren.Add(selectBehaviourNode); // Child = Select behaviour node
        selectBehaviourNode.NodeChildren.Add(HealthPotionBehaviour()); // Child = Health potion behaviour node
        selectBehaviourNode.NodeChildren.Add(SimpleEnemyBehaviour()); // Child = Simple enemy behaviour node
        selectBehaviourNode.NodeChildren.Add(SimpleAllyBehaviour()); // Child = Simple ally behaviour node
        selectBehaviourNode.NodeChildren.Add(BlockadeBehaviour()); // Child = Blockade behaviour node
        selectBehaviourNode.NodeChildren.Add(HeadToEnemyBaseBehaviour()); // Child = Head to enemy base behaviour node
    }

    override internal void TraverseTree()
    {
        // Call current node process node()
        Node.NodeStatus status = StartNode.ProcessNode();
        //Debug.Log("Status = " + status);
    }

    internal override void ChangeTargetRef(GameObject target)
    {
        TargetRef = target;
    }
}
