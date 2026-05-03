using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> slots = new List<InventorySlot>();

    //starting gold and weapon
    public int gold = 100; 
    public Item startingWeapon;

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
    }

    void Start(){
        Load();
        
        //add starting weapon at the start of the game, slots are empty
        if (slots.Count == 0)
        {
            AddItem(startingWeapon);
        }
    }

    //save gold amount, items in slots and their amounts
    public void SaveInventory()
    {
        string path = Application.persistentDataPath + "/save.txt";

        string text = "";

        text += "gold:" + InventoryManager.Instance.gold + "\n";

        foreach (InventorySlot slot in InventoryManager.Instance.slots)
        {
            text += slot.item.itemName + ":" + slot.amount + "\n";
        }

        File.WriteAllText(path, text);
    }

    //load inventory
    public void Load()
    {
        string path = Application.persistentDataPath + "/save.txt";
        if (!File.Exists(path))
        {
            Debug.Log("No save file");
            return;
        }

        //read from file and seperate lines
        string[] lines = File.ReadAllLines(path);

        InventoryManager.Instance.slots.Clear();

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');

            if (parts.Length != 2) continue;

            string key = parts[0];
            int value = int.Parse(parts[1]);

            //load gold
            if (key == "gold")
            {
                InventoryManager.Instance.gold = value;
            }
            else
            {
                Item item = FindItem(key);

                //add item to inventory as an inventory slot and item's prefab to weaponmanager - for fighting
                if (item != null)
                {
                    InventoryManager.Instance.slots.Add(new InventorySlot(item, value));
                    WeaponManager.Instance.weapons.Add(item.prefab);
                }
            }
        }

        Debug.Log("Loaded!");
    }
    
    //loads all items, if names correspond, it will return the item
    public Item FindItem(string itemName)
    {
        Item[] items = Resources.LoadAll<Item>("Items");

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
            if (slot.item == item)
            {
                if(slot.amount < item.maxStack) 
                {
                    return;
                }
                    
                slot.amount++;
                //Debug.Log("Stacked: " + item.itemName + " (" + slot.amount + ")");
                
                SaveInventory();
                return;
            }
        }

        for(int i=0;i<slots.Count;i++)
        {
            if(slots[i].item.type==item.type)
            {
                ChangeWeapon(item, i);
                SaveInventory();
                return;
            }
        }
        slots.Add(new InventorySlot(item, 1));
        WeaponManager.Instance.weapons.Add(item.prefab);
        SaveInventory();
    }

    public void ChangeWeapon(Item replace, int index)
    {
        slots[index]=new InventorySlot(replace, 1);
        WeaponManager.Instance.weapons[index]= replace.prefab;
    }

    public bool CanAddItem(Item item) 
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                return slot.amount < item.maxStack;
            }
        }

        return true;
    }
}