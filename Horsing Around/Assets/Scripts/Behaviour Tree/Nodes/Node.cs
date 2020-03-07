using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    protected bool IsLeaf { get; set; }
    internal List<Node> NodeChildren = new List<Node>(); // List of children
    internal enum NodeStatus { Running, Failure, Success } 
    internal NodeStatus CurrentNodeStatus { get; set; }

    // Child classes should define how to process that node
    abstract internal NodeStatus ProcessNode();
}
