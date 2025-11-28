using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public UnityEvent ItemChanged; 

    public void AddItem(Item item, int amount = 1)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    slot.amount += amount;
                    return;
                }
            }
        }
        /*
        if (slots.Count < inventorySize)
        {
            slots.Add(new InventorySlot(item, amount));
        }
        */
        else
        {
            Debug.Log("inventory full");
        }
        ItemChanged?.Invoke();
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].amount -= amount;

                if (slots[i].amount <= 0)
                    slots.RemoveAt(i);

                return;
            }
        }
        ItemChanged?.Invoke();
    }
}
