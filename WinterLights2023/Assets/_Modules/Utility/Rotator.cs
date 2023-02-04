using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 5.0f;
    [SerializeField] private ControlAxis controlAxis;

    void Update()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        float deltaRotation = (Time.deltaTime * rotateSpeed);
        if (controlAxis == ControlAxis.X)
        {
           rotation.x = (rotation.x + deltaRotation) % 360;
        }
        else if (controlAxis == ControlAxis.Y)
        {
            rotation.y = (rotation.y + deltaRotation) % 360;
        }
        else if (controlAxis == ControlAxis.Z)
        {
            rotation.z = (rotation.z + deltaRotation) % 360;
        }
        transform.rotation = Quaternion.Euler(rotation);
    }
}
