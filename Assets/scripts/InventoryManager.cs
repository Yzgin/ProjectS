using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; // Reference to the inventory panel

    private bool isInventoryOpen = false;

    void Update()
    {
        // Toggle inventory on key press (e.g., "I")
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);
        }
    }

    // Example method to add items to inventory (more complex logic needed for real inventory management)
    public void AddItemToInventory(GameObject item)
    {
        // Implement logic to add item to the inventory UI
        // For example, create a new UI element representing the item and add it to the inventory panel
    }
}
