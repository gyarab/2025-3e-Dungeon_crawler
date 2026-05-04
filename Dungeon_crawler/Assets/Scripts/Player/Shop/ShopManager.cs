using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class Shop : MonoBehaviour
{
    private bool playerInRange = false;

    public ShopUI shopUI;

    public Dictionary<WeaponType, List<Item>> itemsByType;

    public Dictionary<WeaponType, int> currentIndex; //int is index of the current weapon that may be bought

    public HashSet<string> soldItems = new HashSet<string>();

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
    public Item iceDaggers;
    public Item iceSpear;
    public Item iceSword;
    public Item lavaDaggers;
    public Item lavaSpear;
    public Item lavaSword;

    //for soldout weapons - lazy solution
    public Item emptyWeapon1; 
    public Item emptyWeapon2; 
    public Item emptyWeapon3; 
    public Item emptyWeapon4; 
    public Item emptyWeapon5; 
    public Item emptyWeapon6; 
    public Item emptyWeapon7; 

    void Start()
    {
        string pathItem = Application.persistentDataPath + "/saveitem.txt";
        string pathUpgrade = Application.persistentDataPath + "/saveupgrade.txt";
        
        BuildDictionary();

        if (File.Exists(pathItem) && new FileInfo(pathItem).Length > 0) {
            LoadSoldItems();
        }

        if (File.Exists(pathUpgrade) && new FileInfo(pathUpgrade).Length > 0) {
            LoadUpgrade();
        }
        else
        {
            //save initial dictionary if there is no file or it is empty
            SaveUpgrades();
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

        iceDaggers = InventoryManager.Instance.FindItem("IceDaggers");
        iceSpear = InventoryManager.Instance.FindItem("IceSpear");
        iceSword = InventoryManager.Instance.FindItem("IceSword");
        lavaDaggers = InventoryManager.Instance.FindItem("LavaDaggers");
        lavaSpear = InventoryManager.Instance.FindItem("LavaSpear");
        lavaSword = InventoryManager.Instance.FindItem("LavaSword");
        //for soldout weapons
        emptyWeapon1 = InventoryManager.Instance.FindItem("EmptyWeapon1");
        emptyWeapon2 = InventoryManager.Instance.FindItem("EmptyWeapon2");
        emptyWeapon3 = InventoryManager.Instance.FindItem("EmptyWeapon3");
        emptyWeapon4 = InventoryManager.Instance.FindItem("EmptyWeapon4");
        emptyWeapon5 = InventoryManager.Instance.FindItem("EmptyWeapon5");
        emptyWeapon6 = InventoryManager.Instance.FindItem("EmptyWeapon6");
        emptyWeapon7 = InventoryManager.Instance.FindItem("EmptyWeapon7");


        itemsByType[WeaponType.Sword] = new List<Item>
        {
            woodenKatana,
            overgrownSword,
            crystalKatana,
            crystalSword,
            iceSword,
            lavaSword,
            emptyWeapon1
        };

        itemsByType[WeaponType.Axe] = new List<Item>
        {
            overgrownAxe,
            emptyWeapon2
        };

        itemsByType[WeaponType.Hammer] = new List<Item>
        {
            woodenBat,
            emptyWeapon3
        };

        itemsByType[WeaponType.Spear] = new List<Item>
        {
            overgrownSpear,
            crystalSpear,
            iceSpear,
            lavaSpear,
            emptyWeapon4
        };

        itemsByType[WeaponType.Daggers] = new List<Item>
        {
            boneDaggers,
            crystalDaggers,
            iceDaggers,
            lavaDaggers,
            emptyWeapon5
        };

        itemsByType[WeaponType.ShortRange] = new List<Item>
        {
            overgrownSickle,
            emptyWeapon6
        };

        itemsByType[WeaponType.Mace] = new List<Item>
        {
            overgrownMace,
            crystalMace,
            emptyWeapon7
        };

        currentIndex = new Dictionary<WeaponType, int>();

        //at start, index of current weapon type is 0
        foreach (WeaponType type in itemsByType.Keys)
        {
            currentIndex[type] = 0;
        }
    }

    void SaveUpgrades()
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
    public void Upgrade(Item item) 
    {
        WeaponType type = item.type;

        int current = currentIndex[type];

        if (current < itemsByType[type].Count - 1)
        {
            currentIndex[type]++;
        }

        SaveUpgrades();
    }

    //load file by separating lines, WeaponType:index
    void LoadUpgrade()
    {
        string path = Application.persistentDataPath + "/saveupgrade.txt";

        if (!File.Exists(path)) return;

        foreach (WeaponType type in itemsByType.Keys)
        {
            currentIndex[type] = 0;
        }

        //currentIndex = new Dictionary<WeaponType, int>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');

            WeaponType type = (WeaponType)System.Enum.Parse(typeof(WeaponType), parts[0]);
            int index = int.Parse(parts[1]);

            currentIndex[type] = index;
        }
    }

    //saving items to file, one item per line
    public void SaveSoldItems() 
    {
        string path = Application.persistentDataPath + "/saveitem.txt";

        string text = "";

        foreach (string name in soldItems)
        {
            text += name + "\n";
        }

        File.WriteAllText(path, text);
    }

    //adding item to hashset of already sold items
    public void SoldOut(Item item)
    {
        soldItems.Add(item.itemName);
        SaveSoldItems();
    }

    //loading each sold item's name
    public void LoadSoldItems()
    {
        string path = Application.persistentDataPath + "/saveitem.txt";

        soldItems = new HashSet<string>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            soldItems.Add(line);
        }
    }

    //after buying a weapon it cannot be baught again
    public void BuyWeapon(Item item)
    {    
        //does not buy unless there is enough gold and item's stack is not its max
        if (InventoryManager.Instance.gold >= item.price && InventoryManager.Instance.CanAddItem(item))
        {
            InventoryManager.Instance.gold -= item.price;
            InventoryManager.Instance.AddItem(item);

            SoldOut(item);
            shopUI.RefreshItemRows();
        }
    }

    public void BuyUpgrade(Item item)
    {    
        //does not buy unless there is enough gold and item's stack is not its max
        if (InventoryManager.Instance.gold >= item.price && InventoryManager.Instance.CanAddItem(item))
        {
            InventoryManager.Instance.gold -= item.price;
            InventoryManager.Instance.AddItem(item);

            Upgrade(item);
            shopUI.RefreshUpgradeRows();
        }
    }

    public Item GetCurrentWeapon(WeaponType type) 
    {
        int index = currentIndex[type];

        List<Item> list = itemsByType[type];

        if (list == null || list.Count == 0) return null;

        return list[index];
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