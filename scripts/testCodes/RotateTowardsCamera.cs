using UnityEngine;

public class RotateTowardsCameraOnce : MonoBehaviour
{
    // Rotates the character towards the specified camera
    public Transform playerTransform; // The player object or empty orientation object

    void Update()
    {
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        // Find the active camera
        Camera activeCamera = Camera.main; // This will reference the camera tagged as MainCamera

        if (activeCamera != null) // Check if the active camera exists
        {
            // Get the camera's Y rotation
            Vector3 cameraEulerAngles = activeCamera.transform.eulerAngles;
            // Create a new rotation for the player based on the camera's Y rotation
            Quaternion targetRotation = Quaternion.Euler(0, cameraEulerAngles.y, 0);
            // Apply the rotation to the player
            playerTransform.rotation = targetRotation;
        }
    }
}
