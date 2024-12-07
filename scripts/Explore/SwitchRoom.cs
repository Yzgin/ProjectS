using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwitchRoom : MonoBehaviour
{
    public Transform teleportTarget;     // The teleport point to which the player will be moved
    public GameObject player;            // The player object
    public GameObject location;          // The location name UI element
    public Animator doorAnimator;        // The Animator for the current door (door you enter)
    public Image fadePanel;              // The UI panel for fade effect
    public float fadeDuration = 1.0f;    // Duration for fade in/out
    public Camera currentCamera;         // Current active camera
    public Camera nextCamera;            // Camera to switch to after teleporting

    public PlayerMovement playerMovement;

    private bool playerInRange = false;
    private bool interacting = false;

    void Start()
    {
        // Initially hide the location name
        location.SetActive(false);
    }

    void Update()
    {
        // Show location name when the player is near the teleport point
        if (playerInRange && !interacting)
        {
            location.SetActive(true);  // Show location UI when the player is near

            // Start door opening animation and teleportation when "E" is pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                interacting = true;              // Mark as interacting
                location.SetActive(false);       // Hide the location name
                doorAnimator.SetTrigger("OpenDoor"); // Play the "OpenDoor" animation on the current door
            }
        }
    }

    // This function will be called by an animation event after the door opens
    public void TeleportAfterAnimation()
    {
        StartCoroutine(RoomTransition());
    }

    public bool IsInteracting()
    {
        return interacting;
    }
    private IEnumerator RoomTransition()
    {
        // Fade out
        yield return StartCoroutine(Fade(1));

        // Switch the camera to the next one
        currentCamera.gameObject.SetActive(false);
        nextCamera.gameObject.SetActive(true);

        // Teleport the player to the new location
        player.transform.position = teleportTarget.position;

        playerMovement.SetActiveCamera(nextCamera);

        // Fade in
        yield return StartCoroutine(Fade(0));

        // Close the door after teleporting
        doorAnimator.SetTrigger("CloseDoor");

        // Fully reset everything after teleport
        ResetState();
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

    private void ResetState()
    {
        // Reset the interaction status to allow for future interactions
        interacting = false;

        // Make the location UI reappear when the player re-enters the trigger
        if (playerInRange)
        {
            location.SetActive(true);
        }

        // Reset the door animator state to its default state (if needed)
        StartCoroutine(ResetAnimatorState());
    }

    private IEnumerator ResetAnimatorState()
    {
        // Wait for the closing door animation to finish before resetting triggers
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Reset triggers to ensure the door is ready for the next interaction
        doorAnimator.ResetTrigger("OpenDoor");
        doorAnimator.ResetTrigger("CloseDoor");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Show the location UI again if not interacting
            if (!interacting)
            {
                location.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            location.SetActive(false);  // Hide location name when the player moves away
        }
    }
}
