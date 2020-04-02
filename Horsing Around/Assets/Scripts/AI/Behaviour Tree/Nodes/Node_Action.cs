using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node_Action : Node
{
    internal enum ActionTypeEnum
    {
        AttackTarget, DepositGold, FleeTarget, MoveToTarget, RestUp, SetTarget, UseItem
    }
    ActionTypeEnum ActionType;

    BehaviourTree TreeRef;

    Transform Target;
    

    internal Node_Action SetUpNode(ActionTypeEnum actionType, BehaviourTree tree, Transform target = null)
    {
        IsLeaf = true; // Always a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        ActionType = actionType;        
        TreeRef = tree; // Sets behaviour tree instance

        // Only used if action type = set target
        if(actionType == ActionTypeEnum.SetTarget)
        {
            Target = target;
        }        

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

            case ActionTypeEnum.DepositGold:
                // Add the miner's gold to total gold
                int minerGold = TreeRef.GetCurrentGold();
                TreeRef.GameManagerRef.ChangeCurrentGold(minerGold);

                // Set miner gold to zero
                TreeRef.ChangeGold(-minerGold);
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
                // Chance to restore stamina
                float rand = Random.Range(0f, 1f);

                if(rand <= TreeRef.StaminaRecoveryChance)
                {
                    TreeRef.ChangeStamina(TreeRef.StaminaRecoveryAmount);
                }

                break;

            case ActionTypeEnum.SetTarget:
                // Set target
                Transform temp1 = Target;
                if (temp1 == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Set new target failed");
                }
                else
                {
                    TreeRef.ChangeTargetRef(temp1);
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
