using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGuide : MonoBehaviour
{
    public DialogFlow DialogFlowRef;
    public GameObject AIGuidePanel;

    void Start()
    {
        // Turn off panel
        AIGuidePanel.SetActive(false);

        // Make inactive
        DialogFlowRef.isReady = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            // Turn on panel
            AIGuidePanel.SetActive(true);

            // Make active
            DialogFlowRef.isReady = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Turn off panel
            AIGuidePanel.SetActive(false);

            // Make inactive
            DialogFlowRef.isReady = false;
        }
    }
}
