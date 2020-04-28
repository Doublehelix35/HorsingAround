using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    internal int GoldAmount = 1;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            // Increase players gold
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().ChangeCurrentGold(GoldAmount);

            // Destroy coin
            Destroy(gameObject);
        }
    }
}
