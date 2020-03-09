using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Action : Node
{    
    GameObject Target; // Target to perform an action on

    internal enum ActionTypeEnum
    {
        AttackTarget, FleeTarget, MineGold, MoveToTarget, RestUp, UseItem
    }
    ActionTypeEnum ActionType;

    BehaviourTree TreeRef;
    

    internal Node_Action SetUpNode(ActionTypeEnum actionType, BehaviourTree tree, GameObject target)
    {
        IsLeaf = true; // Always a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        ActionType = actionType;
        TreeRef = tree; // Sets behaviour tree instance
        Target = target;

        // Return self
        return this;
    }

    internal override NodeStatus ProcessNode()
    {
        switch (ActionType)
        {
            case ActionTypeEnum.AttackTarget:
                break;
            case ActionTypeEnum.FleeTarget:
                break;
            case ActionTypeEnum.MineGold:
                break;
            case ActionTypeEnum.MoveToTarget:
                break;
            case ActionTypeEnum.RestUp:
                break;
            case ActionTypeEnum.UseItem:
                break;
            default:
                break;
        }

        // Return current node status to parent
        return CurrentNodeStatus;
    }
}
