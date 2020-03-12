using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node_Action : Node
{
    internal enum ActionTypeEnum
    {
        AttackTarget, FleeTarget, MineGold, MoveToTarget, RestUp, SetTargetToClosest, SetTargetToEnemyBase, SetTargetToSighted, UseItem
    }
    ActionTypeEnum ActionType;

    BehaviourTree TreeRef;
    

    internal Node_Action SetUpNode(ActionTypeEnum actionType, BehaviourTree tree)
    {
        IsLeaf = true; // Always a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        ActionType = actionType;        
        TreeRef = tree; // Sets behaviour tree instance

        // Return self
        return this;
    }

    internal override NodeStatus ProcessNode()
    {
        CurrentNodeStatus = NodeStatus.Success;

        switch (ActionType)
        {
            case ActionTypeEnum.AttackTarget:
                // Attack the target
                if (TreeRef.GetTargetRef() != null)
                {
                    // Play attack anim
                    TreeRef.Anim.SetTrigger("IsAttacking");

                    // Update last attack time
                    TreeRef.LastAttackTime = Time.time;

                    // Set attack delay offset to new random value
                    TreeRef.AttackDelayRandOffset = Random.Range(0f, TreeRef.AttackDelayRandOffsetMax);             

                    // Deal damage to target
                    if (TreeRef.GetTargetRef().tag == "Player")
                    {
                        TreeRef.GetTargetRef().GetComponent<Player_Health>().ChangeHealth(-TreeRef.AttackDamage);
                    }
                    else
                    {
                        TreeRef.GetTargetRef().GetComponent<BehaviourTree>().ChangeHealth(-TreeRef.AttackDamage);
                    }

                    Debug.Log("Attack target!");
                }
                else
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Target empty, cant attack target");
                }                
                break;

            case ActionTypeEnum.FleeTarget:
                // Set destination to away from target
                if (TreeRef.GetTargetRef() != null)
                {
                    Vector3 dir = transform.position - TreeRef.GetTargetRef().position;
                    TreeRef.NavAgent.destination = transform.position + (dir.normalized * TreeRef.FleeOffset);
                    Debug.Log("Flee target");
                }
                else
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Target empty, cant flee from target");
                }                
                break;

            case ActionTypeEnum.MineGold:
                break;

            case ActionTypeEnum.MoveToTarget:
                // Set destination to target position
                if(TreeRef.GetTargetRef() != null)
                {
                    TreeRef.NavAgent.destination = TreeRef.GetTargetRef().position;
                }
                else
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Target empty, cant move to target");
                }                
                break;

            case ActionTypeEnum.RestUp:
                break;

            case ActionTypeEnum.SetTargetToClosest:
                // Set target to closest enemy
                Transform temp = TreeRef.Sight.CalculateClosestObject(false).transform;
                if(temp == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Set target to closest failed");
                }
                else
                {
                    TreeRef.ChangeTargetRef(temp);
                }                
                break;

            case ActionTypeEnum.SetTargetToEnemyBase:
                // Set target to enemy base
                Transform temp1 = TreeRef.EnemyBase;
                if (temp1 == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Set target to enemy base failed");
                }
                else
                {
                    TreeRef.ChangeTargetRef(temp1);
                }
                break;

            case ActionTypeEnum.SetTargetToSighted:
                // Set target to closest enemy
                Transform temp2 = TreeRef.Sight.CalculateClosestObject(true).transform;
                if (temp2 == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Set target to sighted failed");
                }
                else
                {
                    TreeRef.ChangeTargetRef(temp2);
                }
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
