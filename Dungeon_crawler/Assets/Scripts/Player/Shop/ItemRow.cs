using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRow : MonoBehaviour
{
    public Shop shop;
    public Item item;
    public GameObject row;

    //row is active if it is not int soldItems
    public void Refresh()
    {
        bool notSold = !shop.soldItems.Contains(item.itemName);
        row.SetActive(notSold);
    }
}