using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMover : MonoBehaviour
{
    [SerializeField] private float speed = 0.05f;
    [SerializeField] private Vector3 minBounds = Vector3.zero;
    [SerializeField] private Vector3 maxBounds = Vector3.zero;

    [SerializeField] private bool sphereCastZOffset = true;
    [SerializeField] private float sphereCastRadius = 0.1f;
    [SerializeField] private float zOffsetDistance = 0.3f;

    //[SerializeField] private Bounds bounds;

    private Vector3 center = new Vector3(0, 0, 4);
    private Vector3 targetPosition = Vector3.zero;

    private RaycastHit hit;

    float x, y, z;
    float counter;

    float zoffset;
    void Update()
    {
        counter += Time.deltaTime * speed;
        x = Mathf.PerlinNoise(0, counter);
        y = Mathf.PerlinNoise(counter, Mathf.PI);
        z = Mathf.PerlinNoise(Mathf.PI*2, counter);

        Vector3 v = new Vector3(
            map(x, 0, 1, minBounds.x, maxBounds.x),
            map(y, 0, 1, minBounds.y, maxBounds.y),
            -30);

        if (sphereCastZOffset)
        {
            if (Physics.SphereCast(v, sphereCastRadius, Vector3.forward, out hit, 100f, 1 << LayerMask.NameToLayer("VFX")) && sphereCastZOffset)
            {
                //Vector3 tempPos = transform.position;
                zoffset = Mathf.Lerp(zoffset, hit.point.z - zOffsetDistance, .05f);
                //transform.position = Vector3.MoveTowards(transform.position, tempPos, 0.05f);
            }
            else
            {
                zoffset = Mathf.Lerp(zoffset, 0, .05f);
            }
        }
        else
            zoffset = 0;

        transform.position = new Vector3(
            map(x,0,1,minBounds.x,maxBounds.x), 
            map(y,0,1,minBounds.y,maxBounds.y), 
            map(z,0,1,minBounds.z+zoffset,maxBounds.z+zoffset)
            );


    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = maxBounds - minBounds;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((minBounds + maxBounds) / 2f, center);
    }

}
