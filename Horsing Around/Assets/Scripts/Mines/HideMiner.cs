using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HideMiner : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Miner")
        {
            other.GetComponent<BT_Miner>().SetVisibility(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Miner")
        {
            other.GetComponent<BT_Miner>().SetVisibility(true);
        }
    }
}
