using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> slots = new List<InventorySlot>();

    public Item testItem;

    public int gold = 100;

    public InventoryUI inventoryUI;

    public ShopUI shopUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (shopUI != null && shopUI.isOpen) 
            {
                return;
            }

            inventoryUI.ToggleInventory();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(Item item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item && slot.amount < item.maxStack)
            {
                slot.amount++;
                //Debug.Log("Stacked: " + item.itemName + " (" + slot.amount + ")");
                return;
            }
        }

        slots.Add(new InventorySlot(item, 1));
        //Debug.Log("Added new: " + item.itemName);
    }
}