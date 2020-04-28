using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Material GreenMat; // Full health material
    public Material RedMat; // Zero health material
    float TransitionPercent = 1.0f; // How much the transition has progressed in %
    Renderer rend; // This gameobjects renderer
    public float WidthMax = 0.8f; // Max size of the health bar

    void Start()
    {
        // Init Values
        rend = GetComponent<Renderer>();
        rend.material = GreenMat;
    }

    void Update()
    {
        // Update health bar
        rend.material.Lerp(GreenMat, RedMat, (1.0f - TransitionPercent)); // Transition from green to red based on health
        transform.localScale = new Vector3(WidthMax * TransitionPercent, 0.1f, 0.1f); // Size relative to health percentage
    }

    public void UpdateTransitionPercentage(float NewPercentage)
    {
        TransitionPercent = NewPercentage;
    }
}
