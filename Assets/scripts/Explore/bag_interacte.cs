using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

public class bag_interacte : MonoBehaviour
{
    public GameObject exclamationMark;   // Reference to the exclamation mark UI element

    private bool playerInRange = false;
    private bool interacting = false;
    public GameObject inventoryPanel; // Reference to the inventory panel

    private bool isInventoryOpen = false;

    void Start()
    {
        exclamationMark.SetActive(false);
        inventoryPanel.SetActive(false);
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
        Check();

    }

    void Check()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
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
            interacting = false;
        }
    }
}
