using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Decision : Node
{
    float ConditionNum;
    float ConditionSucceedNum; // Point at which the condition succeeds
    internal enum DecisionTypeEnum    { HigherToPass, LowerToPass, EqualToPass }
    DecisionTypeEnum DecisionType;


    internal void SetUpNode(DecisionTypeEnum decisionType, float conditionSucceedNum)
    {
        // Init node
        IsLeaf = false; // Cant ever be a leaf node
        DecisionType = decisionType;
        ConditionSucceedNum = conditionSucceedNum;
    }

    internal void UpdateConditionNum(float conditionNum)
    {
        ConditionSucceedNum = conditionNum;
    }

    override internal NodeStatus ProcessNode()
    {
        // Set condition number and succeed num
        //SetConditionNums();

        switch (DecisionType)
        {
            case DecisionTypeEnum.EqualToPass:
                CurrentNodeStatus = ConditionNum == ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            case DecisionTypeEnum.HigherToPass:
                CurrentNodeStatus = ConditionNum >= ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            case DecisionTypeEnum.LowerToPass:
                CurrentNodeStatus = ConditionNum <= ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            default:
                break;
        }

        // Return current node status to parent
        return CurrentNodeStatus;
    }
}
