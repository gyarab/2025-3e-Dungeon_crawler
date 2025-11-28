using UnityEngine;
using System.Collections.Generic;

public class TorchGenerator : MonoBehaviour
{
    public GameObject torchPrefab;
    public float torchChance = 0.05f;

    public void GenerateTorches(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
    {
        foreach (var tile in floorTiles)
        {
            if (Random.value > torchChance)
                continue;

            Vector2Int[] dirs =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            bool nearWall = false;

            foreach (var d in dirs)
            {
                if (wallTiles.Contains(tile + d))
                {
                    nearWall = true;
                    break;
                }
            }

            if (!nearWall) continue;

            Instantiate(torchPrefab, new Vector3(tile.x, tile.y, 0), Quaternion.identity);
        }
    }
}
