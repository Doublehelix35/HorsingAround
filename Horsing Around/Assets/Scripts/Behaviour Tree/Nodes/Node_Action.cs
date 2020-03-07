using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Action : Node
{
    internal enum ActionTypeEnum
    {
        GoToMine
    }
    private ActionTypeEnum ActionType;
    

    internal Node_Action SetUpNode(ActionTypeEnum actionType)
    {
        IsLeaf = true; // Always a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        ActionType = actionType;
        //TreeRef = tree; // Sets target behaviour tree instance

        // Return self
        return this;
    }

    override internal NodeStatus ProcessNode()
    {
        switch (ActionType)
        {
            default:
                break;
        }

        // Return current node status to parent
        return CurrentNodeStatus;
    }
}
