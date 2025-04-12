using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CamRoom7 : MonoBehaviour
{
    public Camera assignedCamera;       // Camera to activate when stepping on the trigger
    public Camera originalCamera;       // Original camera to revert to when stepping off
    public Image fadePanel;             // UI Panel for fade effect
    public float fadeDuration = 1.0f;   // Duration of the fade-in/out effect
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public SwitchRoom[] switchRooms;     // Array of SwitchRoom scripts attached to doors

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(SwitchCameraWithFade(assignedCamera));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning && !AnySwitchRoomInteracting())
        {
            StartCoroutine(SwitchCameraWithFade(originalCamera));
        }
    }

    private bool AnySwitchRoomInteracting()
    {
        foreach (SwitchRoom room in switchRooms)
        {
            if (room != null && room.IsInteracting())  // Check if any SwitchRoom is interacting
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator SwitchCameraWithFade(Camera targetCamera)
    {
        isTransitioning = true;

        // Disable player movement
        playerMovement.enabled = false;
        playerMovement.animator.SetFloat("Speed", 0);

        // Fade out
        yield return StartCoroutine(Fade(1));

        // Disable all cameras
        Camera[] allCameras = Camera.allCameras;
        foreach (Camera cam in allCameras)
        {
            cam.gameObject.SetActive(false);  // Disable all cameras
        }
        targetCamera.gameObject.SetActive(true);  // Enable the target camera

        // Update the active camera in PlayerMovement (this will adjust the player's rotation)
        playerMovement.SetActiveCamera(targetCamera);

        Vector3 moveDirection = playerMovement.transform.forward; // Direction player is facing
        float moveDistance = 1.0f; // Distance to move forward
        float moveSpeed = 3.0f; // Speed of the movement
        float elapsedTime = 0f;

        // Smooth movement to the new position
        Vector3 startPosition = playerMovement.transform.position;
        Vector3 targetPosition = startPosition + moveDirection * moveDistance;

        while (elapsedTime < moveDistance / moveSpeed)
        {
            elapsedTime += Time.deltaTime;
            playerMovement.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / (moveDistance / moveSpeed));
            yield return null;
        }

        // Ensure player reaches the target position
        playerMovement.transform.position = targetPosition;
        // Fade in
        yield return StartCoroutine(Fade(0));

        // Re-enable player movement
        playerMovement.enabled = true;


        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadePanel.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);
    }
}


