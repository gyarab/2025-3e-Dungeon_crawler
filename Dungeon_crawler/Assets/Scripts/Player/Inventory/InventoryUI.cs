using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;

    //public Transform slotsTop; TODO - for normal items
    public Transform slotsBottom; //slots for weapons

    public PlayerMovement playerMovement;

    public bool isOpen = false;

    void Start()
    {
        InventoryManager.Instance.inventoryUI = this;
    }

    public void ToggleInventory()
    {
        if(playerMovement == null && GameManager.Instance != null)
        {
            playerMovement = GameManager.Instance.playerInstance.GetComponent<PlayerMovement>();
        }

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        //handles player's movement
        if (isOpen)
        {
            int itemIndex = 0;

            //UpdateSlots(slotsTop, ref itemIndex);
            UpdateSlots(slotsBottom, ref itemIndex);
            
            playerMovement.moveBlockers.Add(transform);
        }
        else
        {
            if(playerMovement.moveBlockers.Contains(transform))
            {
                playerMovement.moveBlockers.Remove(transform);
            }
        }
    }


    void UpdateUI()
    {
        int itemIndex = 0;

        //UpdateSlots(slotsTop, ref itemIndex);
        UpdateSlots(slotsBottom, ref itemIndex);
    }

    void UpdateSlots(Transform parent, ref int itemIndex)
    {
        //go through all slots in slotsBottom
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform slot = parent.GetChild(i);

            Transform itemImage = slot.Find("ItemImage");
            Transform amountText = slot.Find("AmountText");

            //if the inventory item exists, we get its data from list slots
            if (itemIndex < InventoryManager.Instance.slots.Count)
            {
                InventorySlot slotData = InventoryManager.Instance.slots[itemIndex];

                Image img = itemImage.GetComponent<Image>();
                img.sprite = slotData.item.icon;
                itemImage.gameObject.SetActive(true);

                //item amount shows a number unless it is 1
                if (slotData.amount > 1)
                {
                    amountText.gameObject.SetActive(true);
                    amountText.GetComponent<TMP_Text>().text = slotData.amount.ToString();
                }
                else
                {
                    amountText.gameObject.SetActive(false);
                }

                itemIndex++;
            }
            else
            //if there is not an item to display in the slot, hide the slot's UI
            {
                itemImage.gameObject.SetActive(false);
                amountText.gameObject.SetActive(false);
            }
        }
    }
}