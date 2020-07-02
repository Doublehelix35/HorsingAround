using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyBarricade : MonoBehaviour
{
    GameManager GameManagerRef;
    GameObject barricade;
    GameObject highlighter;

    public bool startActive = false;

    void Start()
    {
        GameManagerRef = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        barricade = transform.GetChild(0).gameObject;
        barricade.SetActive(startActive);
        highlighter = transform.GetChild(1).gameObject;
        highlighter.SetActive(!startActive); // Highlighter is off if barricade is active (vice versa)
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !barricade.activeInHierarchy)
        {
            GameManagerRef.ShowBarricadePanel(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManagerRef.HideBarricadePanel();
        }
    }

    internal void ShowBarricade()
    {
        barricade.SetActive(true);
        highlighter.SetActive(false);
    }
}
