using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense_ObjectsClose : MonoBehaviour
{
    internal int ObjectsClose = 0;
    public string ObjectTag = "Player"; // Tag of the object to spot

    // List of objects close
    List<GameObject> ObjectsCloseList = new List<GameObject>();

    // Calculate and return the closest object
    GameObject CalculateClosestObject()
    {
        // Exit if object close list is empty
        if(ObjectsCloseList.Count <= 0)
        {
            Debug.Log("ERROR! No object close!");
            return new GameObject();
        }

        GameObject closestObject = ObjectsCloseList[0];
        float closestDistance = Vector3.Distance(transform.position, ObjectsCloseList[0].transform.position);

        // Loop through list until closest is found
        foreach (GameObject g in ObjectsCloseList)
        {
            float dist = Vector3.Distance(transform.position, g.transform.position);

            // If dist > closest distance then set closest object to g
            if(dist > closestDistance)
            {
                closestObject = g;
                closestDistance = dist;
            }
        }

        return closestObject;
    }

    void OnTriggerEnter(Collider other)
    {
        // Add object to spotted 
        if (other.gameObject.tag == ObjectTag)
        {
            ObjectsCloseList.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove object from spotted 
        if (other.gameObject.tag == ObjectTag && ObjectsCloseList.Contains(other.gameObject))
        {
            ObjectsCloseList.Remove(other.gameObject);
        }
    }
}
