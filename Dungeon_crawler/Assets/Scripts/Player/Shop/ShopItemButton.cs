using UnityEngine;

public class ShopItemButton : MonoBehaviour
{
    public Item itemToBuy;

    public Shop shop;

    //TODO - ONCLICK for items
    /*public void BuyThisItem()
    {
        shop.BuyItem(itemToBuy);
    }*/

    //ONCLICK for weapons
    public void BuyThisWeapon()
    {
        shop.BuyWeapon(itemToBuy);
    }

    //ONCLICK for upgrades
    public void BuyThisUpgrade()
    {
        shop.BuyUpgrade(itemToBuy);
    }
}