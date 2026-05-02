using UnityEngine;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;

    public GameObject itemsPanel;

    public GameObject upgradesPanel;
    
    public Shop shop;

    public bool isOpen = false;

    public PlayerMovement playerMovement;

    //public List<ShopItemRow> rows = new List<ShopItemRow>();

    //TODO finds correct row by the item
    /*public void SoldOut(Item item)
    {
        foreach (var row in rows)
        {
            if (row.item == item)
            {
                row.SetSoldOut();
                return;
            }
        }
    }*/

    public void ToggleShop()
    {
        isOpen = !isOpen;
        //opens or disables shop
        shopPanel.SetActive(isOpen);

        //blocks player's movement if open
        if (isOpen)
        {
            playerMovement.moveBlockers.Add(gameObject.transform);
            //all upgrade rows refresh using method Refresh() in UpgradeRow
            RefreshAllRows();
        }
        else
        {
            if(playerMovement.moveBlockers.Contains(gameObject.transform)){
                playerMovement.moveBlockers.Remove(gameObject.transform);
            }
        }
    }

    //finds all rows to refresh
    void RefreshAllRows()
    {
        var rows = upgradesPanel.GetComponentsInChildren<UpgradeRow>(true);

        foreach (var row in rows)
        {
            Debug.Log(row.name);
            row.Refresh();
        }
    }

    //only one panel can be active - ONCLICKS on buttons
    public void OpenItems()
    {
        itemsPanel.SetActive(true);
        upgradesPanel.SetActive(false);
    }

    public void OpenUpgrades()
    {
        itemsPanel.SetActive(false);
        upgradesPanel.SetActive(true);
    }

    //DELETE
    /*public void CloseShop()
    {
        shopPanel.SetActive(false);
        isOpen = false;

        if(playerMovement.moveBlockers.Contains(transform))
            playerMovement.moveBlockers.Remove(transform);
    }*/
}