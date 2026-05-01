using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public WeaponType type;
    public GameObject prefab;
    public Sprite icon;
    public int maxStack;
    public int price;
}
