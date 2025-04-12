using UnityEngine;
using UnityEngine.UI;

public class LightSwitchInteraction : MonoBehaviour
{
    public GameObject exclamationMark;   // Reference to the exclamation mark UI element
    public GameObject interactionPanel;  // Reference to the interaction UI Panel
    public Button onButton;              // Reference to the "On" Button
    public Button offButton;             // Reference to the "Off" Button
    public GameObject lightToControl;         // Reference to the light that will be turned on/off
    public MonoBehaviour playerMovementScript; // Reference to the player's movement script

    private bool playerInRange = false;
    private bool interacting = false;

    void Start()
    {
        exclamationMark.SetActive(false);
        interactionPanel.SetActive(false);

        // Add listeners to the buttons to handle the click events
        //lightToControl.SetActive(false);
        onButton.onClick.AddListener(TurnOnLight);
        offButton.onClick.AddListener(TurnOffLight);
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
    }

    void StartInteraction()
    {
        exclamationMark.SetActive(false);
        interactionPanel.SetActive(true);
        interacting = true;

        // Disable player movement
        playerMovementScript.enabled = false;

        // Optionally, you can set the first button as selected to guide the player
        if (lightToControl.activeSelf)
        {
            onButton.Select();
        }
        else
        {
            offButton.Select();
        }

    }

    void TurnOnLight()
    {
        lightToControl.SetActive(true);  // Turn on the light
        EndInteraction();
    }

    void TurnOffLight()
    {
        lightToControl.SetActive(false); // Turn off the light
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
