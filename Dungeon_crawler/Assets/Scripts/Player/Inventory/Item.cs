using UnityEngine;

public enum ItemType
{
    Weapon,
    Consumable,
    Material,
    Quest,
    Miscellaneous
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public bool isStackable = true;
    public ItemType classType;
}
