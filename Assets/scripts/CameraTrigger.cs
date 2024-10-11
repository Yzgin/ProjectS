using UnityEngine;
using System.Collections; // Ensure to include this for coroutines

public class CameraTrigger : MonoBehaviour
{
    public Camera assignedCamera;    // The camera to be activated when the player enters the trigger area
    public Camera[] otherCameras;    // Array of other cameras that should be deactivated

    public float wait = 0.05f;

    public PlayerMovement playerMovement; // Reference to PlayerMovement

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SwitchCameraWithDelay(other));
        }
    }

    private IEnumerator SwitchCameraWithDelay(Collider player)
    {
        // Step 1: Disable player movement
        playerMovement.enabled = false;

        // Step 2: Deactivate all other cameras
        foreach (Camera cam in otherCameras)
        {
            cam.gameObject.SetActive(false);
        }

        // Step 3: Activate the assigned camera
        assignedCamera.gameObject.SetActive(true);

        // Step 4: Set the new active camera in the PlayerMovement script
        player.GetComponent<PlayerMovement>().SetActiveCamera(assignedCamera);

        // Step 5: Wait for 1 second (to keep the player movement disabled)
        yield return new WaitForSeconds(wait);

        // Step 6: Re-enable player movement after the delay
        playerMovement.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Optional: Deactivate the assigned camera here if needed
            assignedCamera.gameObject.SetActive(false);
        }
    }
}
