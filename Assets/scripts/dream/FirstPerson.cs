using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    //public Transform player;
    public float sensitivity = 2f;
    public float maxLeftAngle = -45f; // Set the leftmost limit
    public float maxRightAngle = 45f; // Set the rightmost limit

    public Vector3 intendedCameraRotation; // Intended rotation to move to after pressing E
    public Vector3 intendedCameraPosition; // Intended position to move to after pressing E
    public float lerpSpeed = 5f; // Speed of the smooth transition

    private bool isLocked = false; // Track if the camera movement is locked
    private bool isResetting = false; // Track if the camera is in the process of resetting
    private float currentRotationY = 0f;

    public GameObject deathzone;

    private Vector3 initialCameraPosition; // Store the initial position of the camera
    private Quaternion initialCameraRotation; // Store the initial rotation of the camera

    void Start()
    {

        // Store the initial position and rotation of the camera
        initialCameraPosition = transform.localPosition;
        initialCameraRotation = transform.localRotation;

        // Initialize the current rotation
        currentRotationY = transform.localEulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if "E" is pressed to toggle camera lock/unlock
        if (Input.GetKeyDown(KeyCode.E))
        {

            if (isLocked)
            {
                deathzone.SetActive(false);
                // Unlock the camera: move to the initial position and rotation
                isLocked = false;
                isResetting = true; // Start moving back to the initial position and rotation
            }
            else
            {
                deathzone.SetActive(false);
                // Lock the camera: move to the intended position and rotation
                isLocked = true;
                isResetting = true; // Start moving to the intended position and rotation
            }
        }

        if (isResetting)
        {
            deathzone.SetActive(true);
            SmoothResetPositionAndRotation();
        }
        else if (!isLocked)
        {
            deathzone.SetActive(true);
            // Allow left-right rotation within specified limits
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            currentRotationY += mouseX;

            // Clamp the rotation between the left and right angle limits
            currentRotationY = Mathf.Clamp(currentRotationY, maxLeftAngle, maxRightAngle);

            // Apply rotation only on the Y-axis
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotationY, transform.localEulerAngles.z);
        }
    }

    void SmoothResetPositionAndRotation()
    {
        // Determine target position and rotation based on lock state
        Vector3 targetPosition = isLocked ? intendedCameraPosition : initialCameraPosition;
        Quaternion targetRotation = isLocked ? Quaternion.Euler(intendedCameraRotation) : initialCameraRotation;

        // Smoothly move position towards the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * lerpSpeed);

        // Smoothly transition the rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);

        // Check if close enough to stop the reset
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f &&
            Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
        {
            // Stop resetting once near the target position and rotation
            isResetting = false;
            currentRotationY = intendedCameraRotation.y; // Reset rotation Y for later movement
        }
    }
}
