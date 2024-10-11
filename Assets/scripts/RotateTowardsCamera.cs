using UnityEngine;

public class RotateTowardsCameraOnce : MonoBehaviour
{
    // Rotates the character towards the specified camera
    public void RotateOnce(Camera targetCamera)
    {
        // Calculate direction towards the camera
        Vector3 direction = targetCamera.transform.position - transform.position;
        direction.y = 0;  // Keep the rotation only on the Y axis (horizontal)

        // Apply the rotation once
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
    }
}
