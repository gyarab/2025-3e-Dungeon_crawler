using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PrefabRoom", menuName = "Rooms/Prefab Room")]
public class PrefabRoomSO : ScriptableObject
{
    public GameObject prefab;
    public List<Vector2Int> floors;
    public List<Vector2Int> walls;
    public List<Dictionary<Vector2Int, GameObject>> props = new List<Dictionary<Vector2Int, GameObject>>();

    public BoundsInt bounds;
    public Vector2Int center;

    BoundsInt CalculateUsedBounds(Tilemap tilemap)
    {
        BoundsInt b = tilemap.cellBounds;
        bool hasTile = false;

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (Vector3Int pos in b.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                hasTile = true;
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }
        }

        if (!hasTile)
            return new BoundsInt(0, 0, 0, 0, 0, 0);

        return new BoundsInt(
            new Vector3Int(minX, minY, 0),
            new Vector3Int(maxX - minX + 1, maxY - minY + 1, 1)
        );
    }

    public void extract()
    {
        //extract floors and walls from prefab
        GameObject grid = prefab.transform.GetChild(0).gameObject;
        GameObject wallGrid = grid.transform.GetChild(0).gameObject;
        GameObject floorGrid = grid.transform.GetChild(1).gameObject;

        Tilemap wallTilemap = wallGrid.GetComponent<Tilemap>();
        Tilemap floorTilemap = floorGrid.GetComponent<Tilemap>();

        floors = GetTilePositions(floorTilemap);
        walls = GetTilePositions(wallTilemap);

        //extract props
        GameObject propGrid = prefab.transform.GetChild(1).gameObject;
        foreach (Transform propT in propGrid.transform)
        {
            GameObject prop = propT.gameObject;
            Dictionary<Vector2Int, GameObject> propDict = new Dictionary<Vector2Int, GameObject>();
            Vector2Int pos = new Vector2Int((int)Mathf.Round(prop.transform.position.x), (int)Mathf.Round(prop.transform.position.y));
            propDict.Add(pos, prop);
            props.Add(propDict);
        }

        //extract bounds and center
        BoundsInt wallBounds = CalculateUsedBounds(wallTilemap);
        BoundsInt floorBounds = CalculateUsedBounds(floorTilemap);

        bounds.xMin = Mathf.Min(wallBounds.xMin, floorBounds.xMin);
        bounds.yMin = Mathf.Min(wallBounds.yMin, floorBounds.yMin);
        bounds.xMax = Mathf.Max(wallBounds.xMax, floorBounds.xMax);
        bounds.yMax = Mathf.Max(wallBounds.yMax, floorBounds.yMax);

        center = new Vector2Int((bounds.xMin + bounds.xMax) / 2, (bounds.yMin + bounds.yMax) / 2);
    }

    public List<Vector2Int> GetTilePositions(Tilemap tilemap)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if (tilemap == null) return result;

        BoundsInt b = tilemap.cellBounds;

        foreach (Vector3Int pos in b.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                result.Add((Vector2Int)pos);
            }
        }
        return result;
    }
}
