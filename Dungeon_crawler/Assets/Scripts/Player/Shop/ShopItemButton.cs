using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    public Item itemToBuy;

    public void BuyThisItem()
    {
        if (InventoryManager.Instance.gold >= itemToBuy.price)
        {
            InventoryManager.Instance.gold -= itemToBuy.price;
            InventoryManager.Instance.AddItem(itemToBuy);
        }
    }
}