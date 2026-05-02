using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class Shop : MonoBehaviour
{
    private bool playerInRange = false;

    public ShopUI shopUI;

    //Item[] items = Resources.LoadAll<Item>("Items");

    public Dictionary<WeaponType, List<Item>> itemsByType;

    public Item woodenKatana;
    public Item overgrownSword;
    public Item crystalSword;
    public Item crystalKatana;
    public Item overgrownAxe;
    public Item woodenBat;
    public Item overgrownSpear;
    public Item crystalSpear;
    public Item boneDaggers;
    public Item crystalDaggers;
    public Item overgrownSickle;
    public Item overgrownMace;
    public Item crystalMace;


    void Start()
    {
        //building dictionary only at the start of the game when slots are empty
        if (InventoryManager.Instance.slots.Count == 0) {
            BuildDictionary();
            Save();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !InventoryManager.Instance.inventoryUI.isOpen)
        {
            shopUI.ToggleShop();
        }
    }

    //separating items into lists - manual, because they have specific positions
    void BuildDictionary()
    {
        itemsByType = new Dictionary<WeaponType, List<Item>>();

        woodenKatana = InventoryManager.Instance.FindItem("WoodenKatana");
        if (woodenKatana == null)
        {
            Debug.LogError("WoodenKatana is null!");
        }
        overgrownSword = InventoryManager.Instance.FindItem("OvergrownSword");
        crystalSword = InventoryManager.Instance.FindItem("CrystalSword");
        crystalKatana = InventoryManager.Instance.FindItem("CrystalKatana");
        overgrownAxe = InventoryManager.Instance.FindItem("OvergrownAxe");
        woodenBat = InventoryManager.Instance.FindItem("WoodenBat");
        overgrownSpear = InventoryManager.Instance.FindItem("OvergrownSpear");
        crystalSpear = InventoryManager.Instance.FindItem("CrystalSpear");
        boneDaggers = InventoryManager.Instance.FindItem("BoneDaggers");
        crystalDaggers = InventoryManager.Instance.FindItem("CrystalDaggers");
        overgrownSickle = InventoryManager.Instance.FindItem("OvergrownSickle");
        overgrownMace = InventoryManager.Instance.FindItem("OvergrownMace");
        crystalMace = InventoryManager.Instance.FindItem("CrystalMace");

        itemsByType[WeaponType.Sword] = new List<Item>
        {
            overgrownSword,
            woodenKatana,
            crystalKatana,
            crystalSword
        };

        itemsByType[WeaponType.Axe] = new List<Item>
        {
            overgrownAxe
        };

        itemsByType[WeaponType.Hammer] = new List<Item>
        {
            woodenBat
        };

        itemsByType[WeaponType.Spear] = new List<Item>
        {
            overgrownSpear,
            crystalSpear
        };

        itemsByType[WeaponType.Daggers] = new List<Item>
        {
            boneDaggers,
            crystalDaggers
        };

        itemsByType[WeaponType.ShortRange] = new List<Item>
        {
            overgrownSickle
        };

        itemsByType[WeaponType.Mace] = new List<Item>
        {
            overgrownMace,
            crystalMace
        };
    }

    void Save()
    {
        string path = Application.persistentDataPath + "/saveupgrade.txt";

        string text = "";

        //save dictionary
        foreach (var pair in itemsByType)
        {
            foreach (var item in pair.Value)
            {
                text += pair.Key + ":" + item.itemName + "\n";
            }
        }
        File.WriteAllText(path, text);
    }

    public void SaveUpgrade(Item item) 
    {
        //first we load the dictionary from saveupgrade.txt
        string path = Application.persistentDataPath + "/saveupgrade.txt";
        string[] lines = File.ReadAllLines(path);

        itemsByType = new Dictionary<WeaponType, List<Item>>();

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            WeaponType type = (WeaponType)System.Enum.Parse(typeof(WeaponType), parts[0]);
            Item currentItem = InventoryManager.Instance.FindItem(parts[1]);

            if (!itemsByType.ContainsKey(type))
                itemsByType[type] = new List<Item>();

            itemsByType[type].Add(currentItem);
        }

        //remove the item and save new dictionary
        itemsByType[item.type].Remove(item);
        Save();
        Item newItem = itemsByType[item.type][0];
    }

    //TODO add items to shop
    /*public void BuyItem(Item item)
    {    
        //does not buy unless there is enough gold and item's stack is not its max
        if (InventoryManager.Instance.gold >= item.price && InventoryManager.Instance.CanAddItem(item))
        {
            InventoryManager.Instance.gold -= item.price;
            InventoryManager.Instance.AddItem(item);

            //Debug.Log("Bought: " + item.itemName);
            //Debug.Log("Gold left: " + InventoryManager.Instance.gold);
        }
        else
        {
            //Debug.Log("Not enough gold!");
        }
    }*/

    //TODO after buying a weapon it cannot be baught again
    public void BuyWeapon(Item item)
    {    
        //does not buy unless there is enough gold and item's stack is not its max
        if (InventoryManager.Instance.gold >= item.price && InventoryManager.Instance.CanAddItem(item))
        {
            InventoryManager.Instance.gold -= item.price;
            InventoryManager.Instance.AddItem(item);

            //shopUI.SoldOut(item);
        }
    }

    public void BuyUpgrade(Item item)
    {    
        //does not buy unless there is enough gold and item's stack is not its max
        if (InventoryManager.Instance.gold >= item.price && InventoryManager.Instance.CanAddItem(item))
        {
            InventoryManager.Instance.gold -= item.price;
            InventoryManager.Instance.AddItem(item);

            SaveUpgrade(item);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            //Debug.Log("Press E to buy");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}