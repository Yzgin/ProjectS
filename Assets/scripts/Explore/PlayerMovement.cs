using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float maxEnergy = 100f;
    public float energyDepletionRate = 10f;
    public float energyReplenishRate = 5f;
    public Slider energyBar;
    public GameObject sprintHolder;
    public GameObject Mark;

    public Animator animator;

    private Camera activeCamera; // Reference to the currently active camera

    private float currentEnergy;
    private bool isSprinting;

    void Start()
    {
        currentEnergy = maxEnergy;
        energyBar.maxValue = maxEnergy;
        energyBar.value = currentEnergy;

        // Ensure the energy bar and mark are initially inactive
        energyBar.gameObject.SetActive(false);
        Mark.gameObject.SetActive(false);

        // Set the initial camera (assuming the main camera is active at the start)
        activeCamera = Camera.main;
    }

    void Update()
    {
        HandleSprinting();
        MovePlayer();
        UpdateEnergyBar();
    }

    private void HandleSprinting()
    {
        // Handle sprinting and energy depletion
        if (Input.GetKey(KeyCode.LeftShift) && currentEnergy > 0)
        {
            isSprinting = true;
            currentEnergy -= energyDepletionRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }
        else
        {
            isSprinting = false;
            currentEnergy += energyReplenishRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }
    }

    private void MovePlayer()
    {
        // Determine movement speed based on sprinting status
        float currentSpeed = isSprinting && currentEnergy > 0 ? sprintSpeed : moveSpeed;

        // Get player input for movement (WASD or arrow keys)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Get the active camera's forward and right vectors
        Vector3 forward = activeCamera.transform.forward;
        Vector3 right = activeCamera.transform.right;

        // Project the forward and right vectors onto the XZ plane (ignore Y axis)
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate the movement direction based on the camera orientation
        Vector3 moveDirection = (right * moveX + forward * moveY).normalized;

        // Move the player based on input and current speed
        Vector3 move = moveDirection * currentSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Update animator based on movement input
        animator.SetFloat("Speed", Mathf.Abs(moveX) + Mathf.Abs(moveY));

        // Adjust player and UI orientation based on movement direction
        AdjustPlayerDirection(moveX, moveY);
    }

    private void AdjustPlayerDirection(float moveX, float moveY)
    {
        // Get the forward and right directions of the camera, ignoring Y-axis for 2D-like movement
        Vector3 cameraForward = activeCamera.transform.forward;
        Vector3 cameraRight = activeCamera.transform.right;

        // Ignore the Y-axis to keep movement constrained to the XZ plane
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize to ensure consistent movement speeds
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Get the camera's Y rotation (normalize it between 0-360 degrees)
        float cameraYRotation = NormalizeAngle(activeCamera.transform.eulerAngles.y);

        // Handle Forward/Backward movement based on W and S
        if (moveY != 0)
        {
            if (moveY < 0)  // Moving forward (S)
            {
                // Rotation should be (0, 0, 0) when pressing S (forward)
                transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);
            }
            else if (moveY > 0)  // Moving backward (W)
            {
                // Rotation should be (0, cameraYRotation + 180, 0) when pressing W (backward)
                float newRotationY = (cameraYRotation + 180) % 360; // Ensure it stays within 0-360 range
                transform.rotation = Quaternion.Euler(0, newRotationY, 0);
            }
        }

        // Handle Left/Right movement based on A and D
        if (transform.rotation.eulerAngles.y == cameraYRotation)  // Facing the camera
        {
            if (moveX > 0)  // Moving right (D)
            {
                // Flip character scale for rightward movement
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                Mark.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                sprintHolder.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else if (moveX < 0)  // Moving left (A)
            {
                // Set character scale for leftward movement
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                Mark.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                sprintHolder.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
        }
        else if (transform.rotation.eulerAngles.y == (cameraYRotation + 180) % 360)  // Facing away from the camera
        {
            if (moveX > 0)  // Moving right (D)
            {
                // Flip character scale for rightward movement when facing away
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                Mark.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                sprintHolder.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (moveX < 0)  // Moving left (A)
            {
                // Set character scale for leftward movement when facing away
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                Mark.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                sprintHolder.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    // Helper method to normalize angles between 0-360 degrees
    private float NormalizeAngle(float angle)
    {
        if (angle < 0)
            return angle + 360;
        return angle;
    }




    private void UpdateEnergyBar()
    {
        // Update the energy bar UI value
        energyBar.value = currentEnergy;

        // Show or hide the energy bar based on energy levels and sprinting status
        if (currentEnergy < maxEnergy || isSprinting)
        {
            energyBar.gameObject.SetActive(true);
        }
        else
        {
            energyBar.gameObject.SetActive(false);
        }
    }

    // Method to update the active camera reference
    public void SetActiveCamera(Camera newCamera)
    {
        activeCamera = newCamera;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        playerRotation.y = newCamera.transform.rotation.eulerAngles.y;  // Match Y-rotation with the camera
        transform.rotation = Quaternion.Euler(playerRotation);
    }
}
