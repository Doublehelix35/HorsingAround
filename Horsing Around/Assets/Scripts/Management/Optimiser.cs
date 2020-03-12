using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Optimiser : MonoBehaviour
{
    // Radius of sphere should encompass the whole level at start

    List<GameObject> ObjectsToOptimise;
    string OptimiseTag = "OptimiseMe";
    public float ActivateRadius = 20f;
    public float ActivateRadiusOffset = 20f; // Intial radius offset

    void Start()
    {
        // Init objects to optimise
        ObjectsToOptimise = new List<GameObject>();

        // Fill object list and turn them on/off based on distance
        int layerMask = 1 << 10; // Define bitmask (2nd number is the layer)
        Collider[] hitCols = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, layerMask);

        for(int i = 0; i < hitCols.Length; i++)
        {
            // Add to object list
            ObjectsToOptimise.Add(hitCols[i].gameObject);

            // Check distance
            float dist = Vector3.Distance(transform.position, hitCols[i].transform.position);
            if (dist > ActivateRadius + ActivateRadiusOffset)
            {
                // Deactivate children of object
                if(hitCols[i].transform.childCount > 0)
                {
                    for (int j = 0; j < hitCols[i].transform.childCount; j++)
                    {
                        hitCols[i].transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
                
            }
        }

        // Set sphere radius equal to activate radius
        GetComponent<SphereCollider>().radius = ActivateRadius;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == OptimiseTag)
        {            
            if (ObjectsToOptimise.Contains(col.gameObject))
            {
                Debug.Log("Optimiser col'd and contains " + col.gameObject.name);

                // Turn on all children
                if (col.transform.childCount > 0)
                {
                    for (int i = 0; i < col.transform.childCount; i++)
                    {
                        col.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }               
            }
        }        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == OptimiseTag)
        {            
            if (ObjectsToOptimise.Contains(col.gameObject))
            {
                Debug.Log("Optimiser contains but no longer col'd with " + col.gameObject.name);
                // Turn off all children
                if (col.transform.childCount > 0)
                {
                    for (int i = 0; i < col.transform.childCount; i++)
                    {
                        col.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
