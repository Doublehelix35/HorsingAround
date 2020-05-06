using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	Animator m_Animator;

	void Start ()
	{
		m_Animator = GetComponent<Animator>();
	}

	void OnTriggerEnter (Collider other)
	{
        if(other.tag == "Player" || other.tag == "Enemy" || other.tag == "Miner" || other.tag == "Ally")
        {
            m_Animator.SetBool("Open", true);
            if(other.tag == "Player")
            {
                GetComponent<AudioSource>().Play();
            }            
        }		
	}
	void OnTriggerExit (Collider other)
	{
        if (other.tag == "Player" || other.tag == "Enemy" || other.tag == "Miner" || other.tag == "Ally")
        {
            m_Animator.SetBool("Open", false);
        }
	}
}



