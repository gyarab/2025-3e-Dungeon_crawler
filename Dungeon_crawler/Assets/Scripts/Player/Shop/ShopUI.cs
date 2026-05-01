using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;

    public GameObject itemsPanel;

    public GameObject upgradesPanel;
    
    public Shop shop;

    public bool isOpen = false;

    public PlayerMovement playerMovement;

    public void ToggleShop()
    {
        isOpen = !isOpen;
        shopPanel.SetActive(isOpen);

        if (isOpen)
        {
            playerMovement.moveBlockers.Add(gameObject.transform);
        }
        else
        {
            if(playerMovement.moveBlockers.Contains(gameObject.transform)){
                playerMovement.moveBlockers.Remove(gameObject.transform);
            }
        }
    }

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

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        isOpen = false;

        if(playerMovement.moveBlockers.Contains(transform))
            playerMovement.moveBlockers.Remove(transform);
    }
}