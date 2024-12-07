using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Camera playerCamera;           // Reference to the player's camera
    public Transform pickUpPoint;         // Point where the object will be held when picked up
    public LayerMask pickableLayerMask;   // Layer for pickable objects
    public float interactionDistance = 3f; // Max distance for interacting with objects
    public float pickUpSpeed = 5f;        // Speed of the pick-up transition

    private Transform pickedUpObject = null; // Currently picked-up object
    private Transform originalParent = null; // Original parent of the picked-up object
    private Vector3 originalPosition;        // Original position of the picked-up object
    private Quaternion originalRotation;     // Original rotation of the picked-up object
    private bool isScreenLocked = false;     // Whether the screen is locked

    void Update()
    {
        // Prevent further actions when the screen is locked
        if (isScreenLocked) return;

        // Handle picking up and putting back the object
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (pickedUpObject == null)
            {
                AttemptPickUp();
            }
            else
            {
                PutBackObject();
            }
        }
    }

    private void AttemptPickUp()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, pickableLayerMask))
        {
            Transform target = hit.transform;

            // Save original position, rotation, and parent
            originalPosition = target.position;
            originalRotation = target.rotation;
            originalParent = target.parent;

            // Start smooth pick-up transition
            StartCoroutine(SmoothPickUp(target));
        }
    }

    private IEnumerator SmoothPickUp(Transform target)
    {
        pickedUpObject = target;

        Vector3 initialPosition = target.position;
        Quaternion initialRotation = target.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * pickUpSpeed;
            target.position = Vector3.Lerp(initialPosition, pickUpPoint.position, elapsedTime);
            target.rotation = Quaternion.Lerp(initialRotation, pickUpPoint.rotation, elapsedTime);
            yield return null;
        }

        // Snap to final position and parent the object to the pick-up point
        target.position = pickUpPoint.position;
        target.rotation = pickUpPoint.rotation;
        target.SetParent(pickUpPoint);

        // Lock the screen
        LockScreen();
    }

    private void PutBackObject()
    {
        // Detach and reset the object
        pickedUpObject.SetParent(originalParent);
        pickedUpObject.position = originalPosition;
        pickedUpObject.rotation = originalRotation;
        pickedUpObject = null;

        // Unlock the screen
        UnlockScreen();
    }

    private void LockScreen()
    {
        isScreenLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Disable camera movement (example for direct control disabling)
        if (playerCamera.TryGetComponent(out FirstPerson cameraController)) // Replace with your camera script class
        {
            cameraController.enabled = false;
        }
    }

    private void UnlockScreen()
    {
        isScreenLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Re-enable camera movement
        if (playerCamera.TryGetComponent(out FirstPerson cameraController)) // Replace with your camera script class
        {
            cameraController.enabled = true;
        }
    }
}
