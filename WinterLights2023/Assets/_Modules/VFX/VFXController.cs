using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect currentEffect;

    [SerializeField] private float dragStrength;

    private void SetTurbulenceDrag(float drag)
    {
        currentEffect.SetFloat("Drag", drag);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        SetTurbulenceDrag(Mathf.Sin(Time.time) * dragStrength);
    }
}
