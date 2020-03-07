using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTree : MonoBehaviour
{
    // Variables for branches
    protected GameObject TargetRef; // Ref of object to target

    // Decision node structs
    Node_Decision.DecisionStruct DS_SeeEnemy = new Node_Decision.DecisionStruct(0, 1); // succeed num = min 1 enemy
    Node_Decision.DecisionStruct DS_IsEnemyNear = new Node_Decision.DecisionStruct(0, 1); // succeed num = max 1 unit away


    // Call this to move through the tree
    abstract internal void TraverseTree();

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
        Node_Decision seeEnemy = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.HigherToPass, DS_SeeEnemy);

        // Enemy near selector
        Node_Composite enemyNearSelector = gameObject.AddComponent<Node_Composite>().SetUpNode(Node_Composite.CompositeNodeType.Selector);

        // Check distance to enemy
        Node_Decision isEnemyNear = gameObject.AddComponent<Node_Decision>().SetUpNode(Node_Decision.DecisionTypeEnum.LowerToPass, DS_IsEnemyNear);

        // Move to enemy action
        //BT_Node_Action moveToEnemy = gameObject.AddComponent<BT_Node_Action>().SetUpAction(BT_Node_Action.ActionTypeEnum.MoveToEnemy, this);

        // Attack action
        //BT_Node_Action attackEnemy = gameObject.AddComponent<BT_Node_Action>().SetUpAction(BT_Node_Action.ActionTypeEnum.AttackEnemy, this);

        //Set up children
        // Enemy parent children
        simpleEnemyParent.NodeChildren.Add(seeEnemy); // Child = see enemy decision node
        simpleEnemyParent.NodeChildren.Add(enemyNearSelector); // Child = enemy near selector node
        //simpleEnemyParent.NodeChildren.Add(attackEnemy); // Child = attack enemy action node

        // Enemy near selector children
        enemyNearSelector.NodeChildren.Add(isEnemyNear); // Child = is enemy near decorator node
        //simpleEnemyParent.NodeChildren.Add(moveToEnemy); // Child = move to enemy action node

        return simpleEnemyParent;
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

        // Has potion?

        // Use potion action

        return healthPotionParent;
    }    

}
