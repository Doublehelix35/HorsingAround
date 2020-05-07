using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node_Action : Node
{
    internal enum ActionTypeEnum
    {
        AttackTarget, DepositGold, Disappear, FleeTarget, GiveCommand, MoveToTarget, RestUp, SetCommandersTarget, SetTarget, StealPlayersGold, UseItem
    }
    ActionTypeEnum ActionType;

    BehaviourTree TreeRef;
    GameManager GameManagerRef;

    Transform Target;
    BT_Commander.Commands Command;
    
    internal Node_Action SetUpNode(ActionTypeEnum actionType, BehaviourTree tree, Transform target = null, BT_Commander.Commands command = BT_Commander.Commands.None)
    {
        IsLeaf = true; // Always a leaf node
        CurrentNodeStatus = NodeStatus.Running; // Node is running until it fails or succeeds
        ActionType = actionType;
        TreeRef = tree; // Sets behaviour tree instance

        // Only set if target is used
        if (actionType == ActionTypeEnum.SetTarget || actionType == ActionTypeEnum.SetCommandersTarget)
        {
            Target = target;
        }
        else if (actionType == ActionTypeEnum.GiveCommand)
        {
            Target = target;
            Command = command;
        }

        if(actionType == ActionTypeEnum.StealPlayersGold)
        {
            GameManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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

            case ActionTypeEnum.Disappear:
                // Destroy gameobject
                Destroy(TreeRef.gameObject);
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

            case ActionTypeEnum.GiveCommand:
                // Set target
                Transform ally = Target;
                if (ally == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Give command failed");
                }
                else
                {
                    // Set commander target and give command
                    ally.GetComponent<BehaviourTree>().CommandersTarget = TreeRef.CommandersTarget;
                    ally.GetComponent<BehaviourTree>().ReceiveCommand(Command);
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

            case ActionTypeEnum.SetCommandersTarget:
                // Set target
                Transform temp1 = Target;
                if (temp1 == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Set new commanders target failed");
                }
                else
                {
                    TreeRef.CommandersTarget = temp1;
                }
                break;

            case ActionTypeEnum.SetTarget:
                // Set target
                Transform temp2 = Target;
                if (temp2 == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("Set new target failed");
                }
                else
                {
                    TreeRef.ChangeTargetRef(temp2);
                }
                break;

            case ActionTypeEnum.StealPlayersGold:
                if(GameManagerRef == null)
                {
                    CurrentNodeStatus = NodeStatus.Failure;
                    Debug.Log("GameManager ref is null! Cant steal gold");
                }
                else
                {
                    // Calc gold to steal
                    int goldNeeded = TreeRef.MaxGold - TreeRef.GetCurrentGold();
                    int goldStolen = goldNeeded;
                    bool giveNeededInstead = false; // Alway give enemies gold to reach thier max (even if it doesnt come from player)

                    // Player's current gold
                    int playerGold = GameManagerRef.GetCurrentGold();

                    // Adjust based on players gold available
                    if(playerGold <= 0)
                    {
                        goldStolen = 0;
                        giveNeededInstead = true;
                    }
                    else if(playerGold < goldStolen)
                    {
                        // Steal whatever they have left
                        goldStolen = playerGold;
                        giveNeededInstead = true;
                    }

                    // Take gold from player
                    GameManagerRef.ChangeCurrentGold(-goldStolen);

                    // Give gold
                    if (giveNeededInstead)
                    {
                        TreeRef.ChangeGold(goldNeeded);
                    }
                    else
                    {
                        TreeRef.ChangeGold(goldStolen);
                    }
                    
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
