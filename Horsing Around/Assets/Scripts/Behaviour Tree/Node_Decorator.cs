using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Decorator : Node
{
    internal enum DecoratorNodeType { LimitedRepeater, RepeatTilFail, Reverse, Succeeder, TimerRepeater }
    private DecoratorNodeType decoratorNodeType;

    int LimitedRepeats; // Max number of repeats to be done by a limited repeater
    int RepeatNum = 0; // Current number of repeats
    float TimerLength; // Timer for timer repeater
    float TimerStart; // Time at start of timer
    bool TimerFinished = false;

    internal void SetUpNode(DecoratorNodeType decoratorType, int maxRepeats = 0, float timerLength = 0)
    {
        IsLeaf = false; // Cant ever be a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        decoratorNodeType = decoratorType;
        LimitedRepeats = maxRepeats; // Only used by limited repeater
        TimerLength = timerLength; // Only used by timer repeater
        TimerStart = Time.time; // Init timer
    }

    override internal NodeStatus ProcessNode()
    {
        NodeStatus tempStatus;
        switch (decoratorNodeType)
        {
            // Repeatedly try child node a limited number of times
            case DecoratorNodeType.LimitedRepeater:
                RepeatNum++;
                if (RepeatNum <= LimitedRepeats)
                {
                    tempStatus = NodeChildren[0].ProcessNode();

                    // Always running until all repeats done
                    CurrentNodeStatus = NodeStatus.Running;
                }
                else
                {
                    CurrentNodeStatus = NodeStatus.Success;
                }
                break;

            // Repeatedly try child node until it returns failure
            case DecoratorNodeType.RepeatTilFail:
                // Always running unless it fails
                CurrentNodeStatus = NodeChildren[0].ProcessNode() == NodeStatus.Failure ? NodeStatus.Failure : NodeStatus.Running;
                break;

            // Process child node and reverse result (unless its running)
            case DecoratorNodeType.Reverse:
                tempStatus = NodeChildren[0].ProcessNode();
                if (tempStatus == NodeStatus.Running) // Running
                {
                    CurrentNodeStatus = NodeStatus.Running;
                }
                else if (tempStatus == NodeStatus.Success) // Success
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                }
                else // Failure
                {
                    CurrentNodeStatus = NodeStatus.Success;
                }
                break;

            // Process child node and alway return success
            case DecoratorNodeType.Succeeder:
                NodeChildren[0].ProcessNode();
                CurrentNodeStatus = NodeStatus.Success;
                break;

            // Repeat until timer runs out
            case DecoratorNodeType.TimerRepeater:
                if (!TimerFinished)
                {
                    NodeChildren[0].ProcessNode();
                    CurrentNodeStatus = NodeStatus.Running;
                }
                else
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                }
                break;

            default:
                break;
        }

        // Return current node status to parent
        return CurrentNodeStatus;
    }

    void FixedUpdate()
    {
        if (decoratorNodeType == DecoratorNodeType.TimerRepeater)
        {
            if (Time.time >= TimerStart + TimerLength)
            {
                TimerFinished = true;
            }
        }
    }
}
