using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfRingRotation : MonoBehaviour
{
    public Transform target; // The target object (usually the main camera)
    public Vector3 rotationAxis = Vector3.up; // Axis to rotate around, default is Y-axis
    public float rotationSpeed = 50f; // Speed of rotation

    void Update()
    {
        // Calculate the direction from the object to the target
        Vector3 directionToTarget = target.position - transform.position;

        // Get the current rotation
        Quaternion currentRotation = transform.rotation;

        // Rotate around the specified axis only
        if (rotationAxis == Vector3.up)
        {
            directionToTarget.y = 0; // Ignore Y axis
        }
        else if (rotationAxis == Vector3.right)
        {
            directionToTarget.x = 0; // Ignore X axis
        }
        else if (rotationAxis == Vector3.forward)
        {
            directionToTarget.z = 0; // Ignore Z axis
        }

        // Calculate the new rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Interpolate to the target rotation for smooth movement
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed);
    }
}
