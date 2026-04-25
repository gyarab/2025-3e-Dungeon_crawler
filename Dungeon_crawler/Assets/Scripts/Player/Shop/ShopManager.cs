using UnityEngine;

public class Shop : MonoBehaviour
{
    public Item itemToSell;

    private bool playerInRange = false;

    public ShopUI shopUI;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !InventoryManager.Instance.inventoryUI.isOpen)
        {
            shopUI.ToggleShop();
        }
    }

    public void BuyItem()
    {
        if (InventoryManager.Instance.gold >= itemToSell.price)
        {
            InventoryManager.Instance.gold -= itemToSell.price;
            InventoryManager.Instance.AddItem(itemToSell);

            //Debug.Log("Bought: " + itemToSell.itemName);
            //Debug.Log("Gold left: " + InventoryManager.Instance.gold);
        }
        else
        {
            //Debug.Log("Not enough gold!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            //Debug.Log("Press E to buy");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}