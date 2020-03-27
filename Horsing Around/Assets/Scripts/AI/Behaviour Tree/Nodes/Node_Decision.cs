using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Decision : Node
{
    internal struct DecisionStruct
    {
        internal string Name;
        internal float ConditionNum;
        internal float ConditionSucceedNum; // Number at which the condition succeeds

        internal DecisionStruct(string name, float conditionNum, float conditionSucceedNum)
        {
            Name = name;
            ConditionNum = conditionNum;
            ConditionSucceedNum = conditionSucceedNum;
        }

        internal void SetConditions(float conditionNum, float conditionSucceedNum)
        {            
            ConditionNum = conditionNum;
            ConditionSucceedNum = conditionSucceedNum;
        }
    }

    DecisionStruct DecisionConditions; // Struct that store the decision conditions

    internal enum DecisionTypeEnum    { HigherOrEqualToPass, HigherToPass, LowerOrEqualToPass, LowerToPass, EqualToPass }
    DecisionTypeEnum DecisionType;

    BehaviourTree TreeRef;


    internal Node_Decision SetUpNode(DecisionTypeEnum decisionType, DecisionStruct decisionStruct, BehaviourTree tree)
    {
        // Init node
        IsLeaf = false; // Cant ever be a leaf node
        DecisionType = decisionType;
        DecisionConditions = decisionStruct;
        TreeRef = tree;

        // Return self
        return this;
    }

    override internal NodeStatus ProcessNode()
    {        
        // Update decision struct
        DecisionConditions = TreeRef.UpdateDecisionStruct(DecisionConditions);

        // Make decision based on decision type and condition nums
        switch (DecisionType)
        {
            case DecisionTypeEnum.EqualToPass:
                CurrentNodeStatus = DecisionConditions.ConditionNum == DecisionConditions.ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            case DecisionTypeEnum.HigherOrEqualToPass:
                CurrentNodeStatus = DecisionConditions.ConditionNum >= DecisionConditions.ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            case DecisionTypeEnum.HigherToPass:
                CurrentNodeStatus = DecisionConditions.ConditionNum > DecisionConditions.ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            case DecisionTypeEnum.LowerOrEqualToPass:
                CurrentNodeStatus = DecisionConditions.ConditionNum <= DecisionConditions.ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            case DecisionTypeEnum.LowerToPass:
                CurrentNodeStatus = DecisionConditions.ConditionNum < DecisionConditions.ConditionSucceedNum ? NodeStatus.Success : NodeStatus.Failure;
                break;
            default:
                break;
        }
        // Return current node status to parent
        return CurrentNodeStatus;
    }
}
