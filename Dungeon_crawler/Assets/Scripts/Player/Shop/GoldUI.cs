using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public TMP_Text goldText;

    void Update()
    {
        goldText.text = InventoryManager.Instance.gold.ToString();
    }
}