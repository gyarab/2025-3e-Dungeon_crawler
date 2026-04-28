using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform slotsTop;
    public Transform slotsBottom;

    public PlayerMovement playerMovement;

    public bool isOpen = false;

    void Start()
    {
        InventoryManager.Instance.inventoryUI = this;
    }

    void Update()
    {
        if(playerMovement == null && GameManager.Instance != null)
        {
            playerMovement = GameManager.Instance.playerInstance.GetComponent<PlayerMovement>();
        }
        
    }

    public void ToggleInventory()
    {
        if(playerMovement == null && GameManager.Instance != null)
        {
            playerMovement =
                GameManager.Instance.playerInstance.GetComponent<PlayerMovement>();
        }

        if(playerMovement == null)
        {
            Debug.LogError("PlayerMovement still null");
            return;
        }

        isOpen = !isOpen;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateUI();
            
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
        if (InventoryManager.Instance == null) return;

        int itemIndex = 0;

        UpdateSlots(slotsTop, ref itemIndex);
        UpdateSlots(slotsBottom, ref itemIndex);
    }

    void UpdateSlots(Transform parent, ref int itemIndex)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform slot = parent.GetChild(i);

            Transform itemImage = slot.Find("ItemImage");
            Transform amountText = slot.Find("AmountText");

            if (itemIndex < InventoryManager.Instance.slots.Count)
            {
                InventorySlot slotData = InventoryManager.Instance.slots[itemIndex];

                Image img = itemImage.GetComponent<Image>();
                img.sprite = slotData.item.icon;
                itemImage.gameObject.SetActive(true);

                if (slotData.amount > 1)
                {
                    amountText.gameObject.SetActive(true);
                    amountText.GetComponent<TMP_Text>().text =
                        slotData.amount.ToString();
                }
                else
                {
                    amountText.gameObject.SetActive(false);
                }

                itemIndex++;
            }
            else
            {
                itemImage.gameObject.SetActive(false);
                amountText.gameObject.SetActive(false);
            }
        }
    }
}