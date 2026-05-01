using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    public Item itemToBuy;

    public Shop shop;

    //ONCLICK
    public void BuyThisItem()
    {
        shop.BuyItem(itemToBuy);
    }
}