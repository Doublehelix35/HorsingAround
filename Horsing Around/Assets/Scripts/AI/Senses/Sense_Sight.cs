using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sense_Sight : MonoBehaviour
{
    internal int ObjectsSpotted = 0;
    public string ObjectTag = "Player"; // Tag of the object to spot
    public float SightCheckFreq = 0.1f; // Frequency of sight checks
    float radius;

    // List of objects spotted
    List<GameObject> ObjectsSpottedList = new List<GameObject>();
    List<GameObject> ObjectsCloseList = new List<GameObject>();

    IEnumerator coroutine;

    void Start()
    {
        radius = GetComponent<SphereCollider>().radius;

        coroutine = RecheckSight();
        StartCoroutine(coroutine);
    }

    IEnumerator RecheckSight()
    {
        while (true)
        {
            yield return new WaitForSeconds(SightCheckFreq);

            // Check if any close objects are now visible
            foreach(GameObject g in ObjectsCloseList)
            {
                // Check for clear line of sight
                RaycastHit hit;
                Vector3 dir = g.transform.position - transform.position;
                if(Physics.Raycast(transform.position, dir, out hit, radius))
                {
                    if(hit.transform.gameObject == g)
                    {
                        // Move g from close list to spotted list
                        ObjectsCloseList.Remove(g);
                        ObjectsSpottedList.Add(g);
                    }
                }
            }

            // Check if any spotted objects aren't visible any more
            foreach (GameObject g in ObjectsSpottedList)
            {
                // Check for clear line of sight
                RaycastHit hit;
                Vector3 dir = g.transform.position - transform.position;
                if (Physics.Raycast(transform.position, dir, out hit, radius))
                {
                    if (hit.transform.gameObject != g)
                    {
                        // Move g from spotted list to close list
                        ObjectsSpottedList.Remove(g);
                        ObjectsCloseList.Add(g);
                    }
                }
            }
            // Update objects spotted count
            ObjectsSpotted = ObjectsSpottedList.Count;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Add object to spotted 
        if(other.gameObject.tag == ObjectTag)
        {
            ObjectsCloseList.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove object from spotted/close
        if (other.gameObject.tag == ObjectTag)
        {
            if (ObjectsSpottedList.Contains(other.gameObject))
            {
                ObjectsSpottedList.Remove(other.gameObject);
            }
            else if (ObjectsCloseList.Contains(other.gameObject))
            {
                ObjectsCloseList.Remove(other.gameObject);
            }
            
        }
    }
}
