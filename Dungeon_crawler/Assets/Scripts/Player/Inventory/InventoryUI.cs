using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    private Inventory inventory;

    private void Start()
    {
        inventory = inventoryPanel.GetComponent<Inventory>();
        inventoryPanel.SetActive(false);
        inventory.ItemChanged.AddListener(UpdateInventory);
    }
    public void ShowInventory()
    {
        inventoryPanel.SetActive(true);
    }
    public void HideInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void UpdateInventory()
    {

    } 
}
