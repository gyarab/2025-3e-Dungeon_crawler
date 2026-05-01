using UnityEngine;

public class Shop : MonoBehaviour
{
    private bool playerInRange = false;

    public ShopUI shopUI;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !InventoryManager.Instance.inventoryUI.isOpen)
        {
            shopUI.ToggleShop();
        }
    }

    public void BuyItem(Item item)
    {    
        //does not buy unless there is enough gold and item's stack is not its max
        if (InventoryManager.Instance.gold >= item.price && InventoryManager.Instance.CanAddItem(item))
        {
            InventoryManager.Instance.gold -= item.price;
            InventoryManager.Instance.AddItem(item);

            //Debug.Log("Bought: " + item.itemName);
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