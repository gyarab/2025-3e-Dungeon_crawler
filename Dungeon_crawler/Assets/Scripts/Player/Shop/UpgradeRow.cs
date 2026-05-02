using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeRow : MonoBehaviour
{
    public Shop shop;
    public Item item;
    public GameObject row;

    public void Refresh()
    {
        //row is only visible if the item is the current weapon
        if (item == shop.GetCurrentWeapon(item.type)) {
            row.SetActive(true);
        } 
        else 
        {
            row.SetActive(false);
        }
    }
}