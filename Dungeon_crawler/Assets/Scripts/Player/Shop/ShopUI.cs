using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;

    public GameObject itemsPanel;

    public GameObject upgradesPanel;
    
    public Shop shop;

    //public RectTransform content;

    public bool isOpen = false;

    public PlayerMovement playerMovement;

    public void ToggleShop()
    {
        isOpen = !isOpen;
        //opens or disables shop
        shopPanel.SetActive(isOpen);

        //blocks player's movement if open
        if (isOpen)
        {
            playerMovement.moveBlockers.Add(gameObject.transform);
            //all upgrade and item rows refresh using their method Refresh()
            RefreshUpgradeRows();
            RefreshItemRows();
        }
        else
        {
            if(playerMovement.moveBlockers.Contains(gameObject.transform)){
                playerMovement.moveBlockers.Remove(gameObject.transform);
            }
        }
    }

    //finds all rows in ItemsPanel to refresh
    public void RefreshItemRows()
    {
        var rows = itemsPanel.GetComponentsInChildren<ItemRow>(true);

        foreach (var row in rows)
        {
            row.Refresh();
        }

        //for the scrollbar resize - DOES NOT WORK..
        //Canvas.ForceUpdateCanvases();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    //finds all rows in UpgradePanel to refresh
    public void RefreshUpgradeRows()
    {
        var rows = upgradesPanel.GetComponentsInChildren<UpgradeRow>(true);

        foreach (var row in rows)
        {
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
        //UpgradesPanel can only be opened if all items in itemsPanel were sold (the amount of them is 6)
        if (shop.soldItems.Count < 6)
        {
            return;
        }
        itemsPanel.SetActive(false);
        upgradesPanel.SetActive(true);
    }
}