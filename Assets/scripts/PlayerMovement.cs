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
        // Get the forward direction of the active camera (ignoring Y-axis for 2D-like movement)
        Vector3 cameraForward = activeCamera.transform.forward;
        cameraForward.y = 0; // We only care about horizontal movement

        // Check if the camera is facing forward (local forward direction) or backward (local reverse direction)
        float cameraFacingDirection = Vector3.Dot(cameraForward, Vector3.forward) > 0 ? 0 : 180;

        // Flip the player's direction based on movement and camera rotation
        if (moveX < 0 && transform.rotation.eulerAngles.y == cameraFacingDirection)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            sprintHolder.transform.localScale = new Vector3(1, 1, 1);
            Mark.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX > 0 && transform.rotation.eulerAngles.y == cameraFacingDirection)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            sprintHolder.transform.localScale = new Vector3(-1, 1, 1);
            Mark.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveX > 0 && transform.rotation.eulerAngles.y == (cameraFacingDirection == 0 ? 180 : 0))
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            sprintHolder.transform.localScale = new Vector3(1, 1, 1);
            Mark.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < 0 && transform.rotation.eulerAngles.y == (cameraFacingDirection == 0 ? 180 : 0))
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            sprintHolder.transform.localScale = new Vector3(-1, 1, 1);
            Mark.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveY < 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraFacingDirection, 0);  // Rotate to forward based on camera
            sprintHolder.transform.rotation = Quaternion.Euler(0, 0, 0);
            Mark.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveY > 0)
        {
            transform.rotation = Quaternion.Euler(0, cameraFacingDirection + 180, 0);  // Rotate to backward based on camera
            sprintHolder.transform.rotation = Quaternion.Euler(0, 0, 0);
            Mark.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
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
    }
}
