using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class Shop : MonoBehaviour
{
    private bool playerInRange = false;

    public ShopUI shopUI;

    public Dictionary<WeaponType, List<Item>> itemsByType;

    public Dictionary<WeaponType, int> currentIndex;

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
        //building dictionary only at the very start of the game when all slots are empty
        if (InventoryManager.Instance.slots.Count == 0) {
            BuildDictionary();
            Save();
        }
        else
        {
            //load every time we start the game, so we have all the current info
            Load();
        }
    }

    void Update()
    {
        //opens only if player is nearby and inventory is not open at the same time
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !InventoryManager.Instance.inventoryUI.isOpen)
        {
            shopUI.ToggleShop();
        }
    }

    void BuildDictionary()
    {
        //separating items into lists - manual, because they have specific positions in lists
        itemsByType = new Dictionary<WeaponType, List<Item>>();

        woodenKatana = InventoryManager.Instance.FindItem("WoodenKatana");
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

        currentIndex = new Dictionary<WeaponType, int>();

        //at start, index of current weapon type is 0
        foreach (WeaponType type in itemsByType.Keys)
        {
            currentIndex[type] = 0;
        }
    }

    void Save()
    {
        string path = Application.persistentDataPath + "/saveupgrade.txt";

        string text = "";

        //save dictionary
        foreach (var pair in currentIndex)
        {
            text += pair.Key + ":" + pair.Value + "\n";
        }

        File.WriteAllText(path, text);
    }

    //moves index to the next item, meaning the given item wont be used again
    public void SaveUpgrade(Item item) 
    {
        WeaponType type = item.type;

        int current = currentIndex[type];

        if (current < itemsByType[type].Count - 1)
        {
            currentIndex[type]++;
        }

        Save();
    }

    //load file by separating lines, WeaponType:index
    void Load()
    {
        string path = Application.persistentDataPath + "/saveupgrade.txt";

        if (!File.Exists(path)) return;

        currentIndex = new Dictionary<WeaponType, int>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');

            WeaponType type = (WeaponType)System.Enum.Parse(typeof(WeaponType), parts[0]);
            int index = int.Parse(parts[1]);

            currentIndex[type] = index;
        }
    }

    public Item GetCurrentWeapon(WeaponType type) 
    {
        int index = currentIndex[type];

        List<Item> list = itemsByType[type];

        if (list == null || list.Count == 0) return null;

        return list[index];
    }

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

    //player is in range - can open shop
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
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
}