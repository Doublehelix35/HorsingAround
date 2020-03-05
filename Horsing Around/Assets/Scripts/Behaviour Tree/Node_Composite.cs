using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Composite : Node
{
    internal enum CompositeNodeType { Concurrent, Sequence, Selector, Random }
    private CompositeNodeType CompNodeType;


    internal void SetUpNode(CompositeNodeType compositeType)
    {
        IsLeaf = false; // Cant ever be a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        CompNodeType = compositeType; // Set what kind of composite node it is
    }

    override internal NodeStatus ProcessNode()
    {
        switch (CompNodeType)
        {
            case CompositeNodeType.Concurrent:
                // Current status = how successful concurrent node has been
                CurrentNodeStatus = ConcurrentProcess();
                break;
            case CompositeNodeType.Random:
                // Current status = how successful random node has been
                CurrentNodeStatus = RandomProcess();
                break;
            case CompositeNodeType.Selector:
                // Current status = how successful selector node has been
                CurrentNodeStatus = SelectorProcess();
                break;
            case CompositeNodeType.Sequence:
                // Current status = how successful sequence node has been
                CurrentNodeStatus = SequenceProcess();
                break;
            default:
                Debug.Log("Error! Composite node type not found!");
                CurrentNodeStatus = NodeStatus.Failure;
                break;
        }

        // Return current node status to parent
        return CurrentNodeStatus;
    }

    // Process all node children concurrently until all successful or one fails
    NodeStatus ConcurrentProcess()
    {
        bool isRunning = false;

        for (int i = 0; i < NodeChildren.Count; i++)
        {
            NodeStatus tempStatus = NodeChildren[i].ProcessNode();

            // return fail if any child fails or return running if any still running
            if (tempStatus == NodeStatus.Failure)
            {
                // Return status
                return tempStatus;
            }
        }
        // Return success or running (shouldnt get here if failure)
        return isRunning ? NodeStatus.Running : NodeStatus.Success;
    }

    // Process a random node
    NodeStatus RandomProcess()
    {
        int rand = Random.Range(0, NodeChildren.Count);

        // if node child not null then process random node
        return NodeChildren[rand] != null ? NodeChildren[rand].ProcessNode() : NodeStatus.Failure;
    }

    // Process one node at a time until one succeeds or all fail
    NodeStatus SelectorProcess()
    {
        for (int i = 0; i < NodeChildren.Count; i++)
        {
            NodeStatus tempStatus = NodeChildren[i].ProcessNode();

            // Return if success or running
            if (tempStatus != NodeStatus.Failure)
            {
                // Return status
                return tempStatus;
            }
        }
        // If all nodes fail return failure
        return NodeStatus.Failure;
    }

    // Process one node at a time until all succeeds or one fails
    NodeStatus SequenceProcess()
    {
        for (int i = 0; i < NodeChildren.Count; i++)
        {
            NodeStatus tempStatus = NodeChildren[i].ProcessNode();

            // return fail if any child fails or return running if any still running
            if (tempStatus != NodeStatus.Success)
            {
                // Return status
                return tempStatus;
            }
        }
        // Should only get here if all are successful
        return NodeStatus.Success;
    }
}
