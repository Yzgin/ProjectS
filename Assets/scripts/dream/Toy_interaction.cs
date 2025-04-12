using UnityEngine;
using System.Collections;

public class Toy_interaction : MonoBehaviour
{
    public float interactRange = 5f;
    public LayerMask interactableLayer;
    public Transform holdPosition; // The position where the object will be held (should be in front of the camera)
    public GameObject interactText;
    public float lerpSpeed = 5f;

    private GameObject heldObject = null;
    private Vector3 originalPosition; // Original position of the interactable object
    private Quaternion originalRotation; // Original rotation of the interactable object
    private bool isHoldingObject = false; // Track if the object is currently being held

    private FirstPerson cameraControlScript; // Reference to the FirstPerson camera control script

    void Start()
    {
        // Find and store the camera control script
        cameraControlScript = Camera.main.GetComponent<FirstPerson>(); // Assuming the camera uses the FirstPerson script
    }

    void Update()
    {
        // Handle object interaction: pick up or drop the object
        CheckForInteractable();

        // If holding the object, smoothly move it to the held position
        if (isHoldingObject && heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            if (!isHoldingObject)
            {
                interactText.SetActive(true);
            }


            // Attempt to pick up the object when pressing F
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isHoldingObject)
                {
                    DropHeldObject(); // Drop the object if already holding it
                }
                else
                {
                    PickupObject(hit.collider.gameObject); // Pickup the object if not holding anything
                }
            }
        }
        else
        {
            interactText.SetActive(false);
        }
    }

    void PickupObject(GameObject obj)
    {
        isHoldingObject = true;
        Debug.Log($"Picking up object: {obj.name}");
        heldObject = obj;

        // Save the original position and rotation
        originalPosition = obj.transform.position;
        originalRotation = obj.transform.rotation;

        // Disable physics to smoothly move the object
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            Debug.Log("Rigidbody set to Kinematic.");
        }

        // Disable camera movement
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = false; // Disable the camera movement
        }

        Toy_system toySystem = heldObject.GetComponent<Toy_system>();
        if (toySystem != null)
        {
            toySystem.SetHeldState(true);
        }
        // Hide interaction text
        interactText.SetActive(false);
    }

    void DropHeldObject()
    {

        if (heldObject == null)
        {
            return;
        }

        Toy_system toySystem = heldObject.GetComponent<Toy_system>();
        if (toySystem != null)
        {
            toySystem.SetHeldState(false);
        }

        Debug.Log($"Dropping object: {heldObject.name}");

        // Start the smooth transition for the object back to its original position and rotation
        StartCoroutine(SmoothDropObject());
    }

    IEnumerator SmoothDropObject()
    {
        float timeElapsed = 0f;
        Vector3 initialPosition = heldObject.transform.position;
        Quaternion initialRotation = heldObject.transform.rotation;

        // Smoothly transition the position and rotation back to the original, with a faster rate
        float dropSpeed = lerpSpeed * 0.1f; // You can adjust the multiplier to control the speed of return

        while (timeElapsed < dropSpeed)
        {
            heldObject.transform.position = Vector3.Lerp(initialPosition, originalPosition, timeElapsed / dropSpeed);
            heldObject.transform.rotation = Quaternion.Lerp(initialRotation, originalRotation, timeElapsed / dropSpeed);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact original position and rotation after the transition
        heldObject.transform.position = originalPosition;
        heldObject.transform.rotation = originalRotation;

        // Re-enable physics
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            Debug.Log("Rigidbody set back to non-Kinematic.");
        }

        // Clear references
        heldObject = null;
        isHoldingObject = false;

        // Re-enable camera movement
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = true; // Re-enable the camera movement
        }

        // Re-enable interaction text
        interactText.SetActive(false);
    }



    void MoveHeldObject()
    {
        // Smoothly move the object to the held position
        heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPosition.position, lerpSpeed * Time.deltaTime);
        heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation, holdPosition.rotation, lerpSpeed * Time.deltaTime);
    }

}
