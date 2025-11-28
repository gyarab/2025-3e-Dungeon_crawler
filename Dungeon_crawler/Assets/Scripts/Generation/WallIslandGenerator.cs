using System.Collections.Generic;
using UnityEngine;

public class WallIslandGenerator : MonoBehaviour
{
    [Range(0f, 1f)] public float noiseScale = 0.1f;
    [Range(0f, 1f)] public float threshold = 0.5f;
    [Range(1, 1000)] public int seed = 0;

    public HashSet<Vector2Int> GenerateWallIslands(HashSet<Vector2Int> roomTiles, HashSet<Vector2Int> corridorTiles = null)
    {
        System.Random rng = new System.Random(seed);
        float offsetX = rng.Next(-10000, 10000);
        float offsetY = rng.Next(-10000, 10000);

        HashSet<Vector2Int> islands = new HashSet<Vector2Int>();

        foreach (var tile in roomTiles)
        {
            if (corridorTiles != null && corridorTiles.Contains(tile))
                continue;

            float sample = Mathf.PerlinNoise(tile.x * noiseScale + offsetX, tile.y * noiseScale + offsetY);
            if (sample > threshold)
                islands.Add(tile);
        }

        return islands;
    }
}
