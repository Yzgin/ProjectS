using System.Collections.Generic;
using UnityEngine;

public class SwitchRoom : MonoBehaviour
{
    public Transform teleportTarget;     // The teleport point to which the player will be moved
    public GameObject player;            // The player object
    public GameObject location;          // The location name UI element
    public Animator doorAnimator;        // The Animator for the current door (door you enter)


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
                //Debug.Log("Opening door...");
                interacting = true;              // Mark as interacting
                location.SetActive(false);       // Hide the location name
                doorAnimator.SetTrigger("OpenDoor"); // Play the "OpenDoor" animation on the current door
            }
        }
    }

    // This function will be called by an animation event after the door opens
    public void TeleportAfterAnimation()
    {
        // Teleport the player to the new location
        player.transform.position = teleportTarget.position;

        //Debug.Log("Teleporting player and closing door...");

        // Close the door after teleporting
        doorAnimator.SetTrigger("CloseDoor");  // Trigger "CloseDoor" animation

        // Fully reset everything after teleport
        ResetState();
    }

    // Reset the door interaction state to allow for future interaction
    void ResetState()
    {
        // Debugging reset process
        //Debug.Log("Resetting interaction state...");

        // Reset the interaction status to allow for future interactions
        interacting = false;

        // Make the location UI reappear when the player re-enters the trigger
        if (playerInRange)
        {
            location.SetActive(true);
        }

        // Reset the door animator state to its default state (if needed)
        // Wait for the door to finish closing before resetting the state.
        StartCoroutine(ResetAnimatorState());
    }

    private System.Collections.IEnumerator ResetAnimatorState()
    {
        // Wait for the closing door animation to finish before resetting triggers
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Reset triggers to ensure the door is ready for the next interaction
        doorAnimator.ResetTrigger("OpenDoor");
        doorAnimator.ResetTrigger("CloseDoor");

        //Debug.Log("Animator reset complete.");
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
