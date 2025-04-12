using UnityEngine.UI;
using UnityEngine;

public class deskTrigger : MonoBehaviour
{
    public GameObject exclamationMark;   // Reference to the exclamation mark UI element
    public GameObject interactionPanel;  // Reference to the interaction UI Panel
    public GameObject inventory;         // Reference to the inventory UI
    public Button Lamp;                  // Reference to the "Lamp" Button
    public Button desk;                  // Reference to the "Desk" Button
    public GameObject lightToControl;    // Reference to the light that will be turned on/off
    public MonoBehaviour playerMovementScript; // Reference to the player's movement script

    private bool playerInRange = false;
    private bool interacting = false;
    private bool isInventoryOpen = false;

    void Start()
    {
        exclamationMark.SetActive(false);
        interactionPanel.SetActive(false);
        lightToControl.SetActive(false);

        // Add listeners to the buttons to handle the click events
        Lamp.onClick.AddListener(LightControl);
        desk.onClick.AddListener(BagCheck);
    }

    void Update()
    {
        if (playerInRange && !interacting)
        {
            exclamationMark.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartInteraction();
            }
        }

        // If inventory is open, allow the player to close it by pressing 'E' or 'Escape'
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    void StartInteraction()
    {
        exclamationMark.SetActive(false);
        interactionPanel.SetActive(true);
        interacting = true;

        // Disable player movement
        playerMovementScript.enabled = false;

        // Optionally, you can set the first button as selected to guide the player
        Lamp.Select();
    }

    void LightControl()
    {
        if (lightToControl.activeSelf)
        {
            lightToControl.SetActive(false);  // Turn off the light
        }
        else
        {
            lightToControl.SetActive(true);   // Turn on the light
        }

        EndInteraction();
    }

    void BagCheck()
    {
        isInventoryOpen = true;
        inventory.SetActive(true);
    }

    void CloseInventory()
    {
        isInventoryOpen = false;
        inventory.SetActive(false);
        EndInteraction();
    }

    void EndInteraction()
    {
        interactionPanel.SetActive(false);
        interacting = false;

        // Re-enable player movement
        playerMovementScript.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            exclamationMark.SetActive(false);
            interactionPanel.SetActive(false);
            interacting = false;

            // Ensure player movement is re-enabled if the player leaves the trigger while interacting
            playerMovementScript.enabled = true;
        }
    }
}
