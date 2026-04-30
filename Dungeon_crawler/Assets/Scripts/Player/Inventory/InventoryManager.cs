using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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
    }
    else
    {
        Destroy(gameObject); 
    }
        Load();
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/save.txt";

        string text = "";

        // save gold
        text += "gold:" + InventoryManager.Instance.gold + "\n";

        // save items
        foreach (InventorySlot slot in InventoryManager.Instance.slots)
        {
            text += slot.item.itemName + ":" + slot.amount + "\n";
        }

        File.WriteAllText(path, text);

        Debug.Log("Saved: " + path);
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/save.txt";
        if (!File.Exists(path))
        {
            Debug.Log("No save file");
            return;
        }

        string[] lines = File.ReadAllLines(path);

        InventoryManager.Instance.slots.Clear();

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');

            if (parts.Length != 2) continue;

            string key = parts[0];
            int value = int.Parse(parts[1]);

            if (key == "gold")
            {
                InventoryManager.Instance.gold = value;
            }
            else
            {
                Item item = FindItem(key);

                if (item != null)
                {
                    InventoryManager.Instance.slots.Add(new InventorySlot(item, value));
                }
                else
                {
                    Debug.LogWarning("Item not found: " + key);
                }
            }
        }

        Debug.Log("Loaded!");
    }
    
    Item FindItem(string itemName)
    {
        Item[] items = Resources.LoadAll<Item>("");

        foreach (Item item in items)
        {
            if (item.itemName == itemName)
                return item;
        }

        return null;
    }

    public void AddItem(Item item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item && slot.amount < item.maxStack)
            {
                slot.amount++;
                //Debug.Log("Stacked: " + item.itemName + " (" + slot.amount + ")");
                
                Save();
                return;
            }
        }

        slots.Add(new InventorySlot(item, 1));
        //Debug.Log("Added new: " + item.itemName);
    }
}