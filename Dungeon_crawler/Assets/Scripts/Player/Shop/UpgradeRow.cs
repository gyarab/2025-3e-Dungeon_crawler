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
        Item current = shop.GetCurrentWeapon(item.type);
        row.SetActive(current != null && current == item);
    }
}