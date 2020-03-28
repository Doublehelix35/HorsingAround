using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSwitch : MonoBehaviour
{
    public GameObject OpenChest;
    public GameObject ClosedChest;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Miner")
        {
            // Show open chest, hide closed chest
            OpenChest.SetActive(true);
            ClosedChest.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Miner")
        {
            // Show closed chest, hide open chest        
            ClosedChest.SetActive(true);
            OpenChest.SetActive(false);
        }
    }
}
