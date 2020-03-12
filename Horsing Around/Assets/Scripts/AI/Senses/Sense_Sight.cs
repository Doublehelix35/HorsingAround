using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sense_Sight : MonoBehaviour
{
    internal int ObjectsSpottedCount = 0;
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
            if (ObjectsCloseList.Count > 0)
            {                
                for(int i = 0; i < ObjectsCloseList.Count; i++)
                {
                    // Check for clear line of sight
                    RaycastHit hit;
                    Vector3 dir = ObjectsCloseList[i].transform.position - transform.position;
                    if (Physics.Raycast(transform.position, dir, out hit, radius))
                    {
                        if (hit.transform.gameObject == ObjectsCloseList[i])
                        {
                            // Move g from close list to spotted list                            
                            ObjectsSpottedList.Add(ObjectsCloseList[i]);
                            ObjectsCloseList.Remove(ObjectsCloseList[i]);
                        }
                    }
                }
            }

            if(ObjectsSpottedList.Count > 0)
            {
                // Check if any spotted objects aren't visible any more
                for (int i = 0; i < ObjectsSpottedList.Count; i++)
                {
                    // Check for clear line of sight
                    RaycastHit hit;
                    Vector3 dir = ObjectsSpottedList[i].transform.position - transform.position;
                    if (Physics.Raycast(transform.position, dir, out hit, radius))
                    {
                        if (hit.transform.gameObject != ObjectsSpottedList[i])
                        {
                            // Move g from spotted list to close list
                            ObjectsCloseList.Add(ObjectsSpottedList[i]);
                            ObjectsSpottedList.Remove(ObjectsSpottedList[i]);
                        }
                    }
                }
            }          
            
            // Update objects spotted count
            ObjectsSpottedCount = ObjectsSpottedList.Count;
        }
    }

    // Calculate and return the closest object
    internal GameObject CalculateClosestObject(bool HasBeenSpotted)
    {
        // Set temp list to either spotted list or close list
        List<GameObject> tempList = HasBeenSpotted ? ObjectsSpottedList : ObjectsCloseList;

        // Exit if list is empty
        if (tempList.Count <= 0)
        {
            Debug.Log("ERROR! No object close!");
            return null;
        }

        GameObject closestObject = tempList[0];
        float closestDistance = Vector3.Distance(transform.position, tempList[0].transform.position);

        // Loop through list until closest is found
        foreach (GameObject g in tempList)
        {
            float dist = Vector3.Distance(transform.position, g.transform.position);

            // If dist > closest distance then set closest object to g
            if (dist > closestDistance)
            {
                closestObject = g;
                closestDistance = dist;
            }
        }

        return closestObject;
    }

    // Calculate and return the closest object
    internal float CalculateClosestObjectDistance(bool HasBeenSpotted)
    {
        // Set temp list to either spotted list or close list
        List<GameObject> tempList = HasBeenSpotted ? ObjectsSpottedList : ObjectsCloseList;

        // Exit if list is empty
        if (tempList.Count <= 0)
        {
            Debug.Log("ERROR! No object close!");
            return 10000;
        }

        float closestDistance = Vector3.Distance(transform.position, tempList[0].transform.position);

        // Loop through list until closest is found
        foreach (GameObject g in tempList)
        {
            float dist = Vector3.Distance(transform.position, g.transform.position);

            // If dist > closest distance then set closest object to g
            if (dist > closestDistance)
            {
                closestDistance = dist;
            }
        }

        return closestDistance;
    }

    void OnTriggerEnter(Collider col)
    {
        // Add object to spotted 
        if(col.gameObject.tag == ObjectTag)
        {
            ObjectsCloseList.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        // Remove object from spotted/close
        if (col.gameObject.tag == ObjectTag)
        {
            if (ObjectsSpottedList.Contains(col.gameObject))
            {
                ObjectsSpottedList.Remove(col.gameObject);
                ObjectsSpottedCount = ObjectsSpottedList.Count;
            }
            else if (ObjectsCloseList.Contains(col.gameObject))
            {
                ObjectsCloseList.Remove(col.gameObject);
            }
            
        }
    }
}
