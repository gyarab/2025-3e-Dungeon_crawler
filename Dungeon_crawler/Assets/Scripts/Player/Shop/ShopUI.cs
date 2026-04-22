using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public GameObject shopPanel;
    
    public Shop shop;

    public bool isOpen = false;

    public PlayerMovement playerMovement;

    public void ToggleShop()
    {
        isOpen = !isOpen;
        shopPanel.SetActive(isOpen);

        if (isOpen)
        {
            playerMovement.canMove = false;
        }
        else
        {
            playerMovement.canMove = true;
        }
    }

    public void Buy()
    {
        shop.BuyItem();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }
}