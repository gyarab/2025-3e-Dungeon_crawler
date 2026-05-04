using UnityEngine;

public class coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Add points to the player's score
            InventoryManager.Instance.gold += 1;
            InventoryManager.Instance.SaveInventory();
            Destroy(gameObject);
        }
    }
}
