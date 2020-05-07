using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sense_Sight : MonoBehaviour
{
    public string[] ObjectTags; // Tags of the objects to spot
    public float SightCheckFreq = 0.1f; // Frequency of sight checks
    float radius;

    // Lists of objects spotted and close
    List<GameObject>[] ObjectsSpottedLists;
    List<GameObject>[] ObjectsCloseLists;

    IEnumerator coroutine;

    void Start()
    {
        // Init lists arrays
        ObjectsSpottedLists = new List<GameObject>[ObjectTags.Length];
        ObjectsCloseLists = new List<GameObject>[ObjectTags.Length];

        // Init each list in arrays
        for(int i = 0; i < ObjectTags.Length; i++)
        {
            ObjectsSpottedLists[i] = new List<GameObject>();
            ObjectsCloseLists[i] = new List<GameObject>();
        }

        radius = GetComponent<SphereCollider>().radius;

        coroutine = RecheckSight();
        StartCoroutine(coroutine);
    }

    IEnumerator RecheckSight()
    {
        while (true)
        {
            yield return new WaitForSeconds(SightCheckFreq);
            // Check for nulls
            CheckListsForNull();

            // Check for each tag
            for (int i = 0; i < ObjectTags.Length; i++)
            {
                // Check if any close objects are now visible
                if (ObjectsCloseLists[i].Count > 0)
                {
                    for (int j = 0; j < ObjectsCloseLists[i].Count; j++)
                    {
                        // Check for clear line of sight
                        RaycastHit hit;
                        Vector3 dir = ObjectsCloseLists[i][j].transform.position - transform.position;
                        if (Physics.Raycast(transform.position, dir, out hit, radius))
                        {
                            // Check if object matches
                            if (hit.transform.gameObject == ObjectsCloseLists[i][j])
                            {
                                // Move g to spotted list (only if its not already in there)
                                if(!ObjectsSpottedLists[i].Contains(ObjectsCloseLists[i][j]))
                                {
                                    ObjectsSpottedLists[i].Add(ObjectsCloseLists[i][j]);
                                }                                
                            }
                            else // No sight
                            {
                                // Make sure not on spotted list
                                if (ObjectsSpottedLists[i].Contains(ObjectsCloseLists[i][j]))
                                {
                                    ObjectsSpottedLists[i].Remove(ObjectsCloseLists[i][j]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal int GetObjectsSpottedCount(string tag)
    {
        // Return the relevant count if tag matches
        for (int i = 0; i < ObjectTags.Length; i++)
        {
            if(tag == ObjectTags[i])
            {
                return ObjectsSpottedLists[i].Count;
            }
        }
        return 0;
    }

    internal int GetObjectsCloseCount(string tag)
    {
        // Return the relevant count if tag matches
        for (int i = 0; i < ObjectTags.Length; i++)
        {
            if (tag == ObjectTags[i])
            {
                return ObjectsCloseLists[i].Count;
            }
        }
        return 0;
    }

    internal List<GameObject> GetObjectSpottedList(string tag)
    {
        // Return relevant list if tag matches
        for(int i = 0; i < ObjectTags.Length; i++)
        {
            if(tag == ObjectTags[i])
            {
                return ObjectsCloseLists[i];
            }
        }
        return null;
    }

    // Calculate and return the closest object
    internal GameObject CalculateClosestObject(string objectTag, bool hasBeenSpotted)
    {
        // Check for nulls
        CheckListsForNull();

        // Set temp lists to either spotted lists or close lists
        List<GameObject>[] tempLists = hasBeenSpotted ? ObjectsSpottedLists : ObjectsCloseLists;

        // Check lists for a tag match
        for (int i = 0; i < ObjectTags.Length; i++)
        {
            if (objectTag == ObjectTags[i])
            {
                // Exit if list is empty
                if (tempLists[i].Count <= 0)
                {
                    Debug.Log("ERROR! No object close!");
                    return null;
                }

                // Start with first object in list
                GameObject closestObject = tempLists[i][0];
                float closestDistance = 10000;

                // Loop through list until closest is found
                for (int j = 0; j < tempLists[i].Count; j++)
                {
                    float dist = Vector3.Distance(transform.position, tempLists[i][j].transform.position);

                    // If dist < closest distance then set closest object to g
                    if (dist < closestDistance)
                    {
                        closestObject = tempLists[i][j];
                        closestDistance = dist;
                    }
                }
                return closestObject;
            }
        }
        Debug.Log("ERROR! No object tag match!");
        return null;
    }

    // Calculate and return the closest object
    internal float CalculateClosestObjectDistance(string objectTag, bool hasBeenSpotted)
    {
        // Check for nulls
        CheckListsForNull();

        // Set temp list to either spotted list or close list
        List<GameObject>[] tempLists = hasBeenSpotted ? ObjectsSpottedLists : ObjectsCloseLists;

        // Check lists for a tag match
        for (int i = 0; i < ObjectTags.Length; i++)
        {
            if (objectTag == ObjectTags[i])
            {
                if (tempLists[i].Count <= 0)
                {
                    Debug.Log("ERROR! No object close to calc distance!");
                    return 10000;
                }

                float closestDistance = 10000;

                // Loop through list until closest is found
                for (int j = 0; j < tempLists[i].Count; j++)
                {
                    float dist = Vector3.Distance(transform.position, tempLists[i][j].transform.position);

                    // If dist < closest distance then set closest object to dist
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                    }

                }
                return closestDistance;
            }
        }
        Debug.Log("ERROR! No object tag match!");
        return 10000;
    }

    void CheckListsForNull()
    {
        // Check and remove any null objects
        for (int i = 0; i < ObjectTags.Length; i++)
        {
            // Check if any close objects are now visible
            if (ObjectsCloseLists[i].Count > 0)
            {
                ObjectsCloseLists[i].RemoveAll(g => g == null);
            }

            // Check if any spotted objects aren't visible any more
            if (ObjectsSpottedLists[i].Count > 0)
            {
                ObjectsSpottedLists[i].RemoveAll(g => g == null);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // Add object to list that matches tag
        for(int i = 0; i < ObjectTags.Length; i++)
        {
            if (col.gameObject.tag == ObjectTags[i])
            {
                ObjectsCloseLists[i].Add(col.gameObject);
            }
        }        
    }

    void OnTriggerExit(Collider col)
    {
        // Remove object from spotted & close
        for (int i = 0; i < ObjectTags.Length; i++)
        {
            if (col.gameObject.tag == ObjectTags[i])
            {
                // Remove from spotted
                if (ObjectsSpottedLists[i].Contains(col.gameObject))
                {
                    ObjectsSpottedLists[i].Remove(col.gameObject);
                }

                // Remove from close
                if (ObjectsCloseLists[i].Contains(col.gameObject))
                {
                    ObjectsCloseLists[i].Remove(col.gameObject);
                }

            }
        }
    }
}
