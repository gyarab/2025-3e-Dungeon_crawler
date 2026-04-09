using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform slotsParent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            if (inventoryPanel.activeSelf)
            {
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        if (InventoryManager.Instance == null) return;

        for (int i = 0; i < slotsParent.childCount; i++)
        {
            Transform slot = slotsParent.GetChild(i);

            Transform itemImage = slot.Find("ItemImage");
            Transform amountText = slot.Find("AmountText");

            if (itemImage == null || amountText == null)
            {
                Debug.LogError("Slot missing ItemImage or AmountText!");
                continue;
            }

            if (i < InventoryManager.Instance.slots.Count)
            {
                InventorySlot slotData = InventoryManager.Instance.slots[i];

                Image img = itemImage.GetComponent<Image>();
                img.sprite = slotData.item.icon;
                itemImage.gameObject.SetActive(true);

                if (slotData.amount > 1)
                {
                    amountText.gameObject.SetActive(true);
                    amountText.GetComponent<TMPro.TMP_Text>().text = slotData.amount.ToString();
                }
                else
                {
                    amountText.gameObject.SetActive(false);
                }
            }
            else
            {
                itemImage.gameObject.SetActive(false);
                amountText.gameObject.SetActive(false);
            }
        }
    }
}