using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PrefabRoom", menuName = "Rooms/Prefab Room")]
public class PrefabRoomSO : ScriptableObject
{
    //premade rooms
    public GameObject prefab;
    public List<Vector2Int> floors = new List<Vector2Int>();
    public List<Vector2Int> walls = new List<Vector2Int>();
    public List<Vector2Int> propPos = new List<Vector2Int>();
    public List<GameObject> propGO = new List<GameObject>();

    public BoundsInt bounds;
    public Vector2Int center;

    public bool hasEnemies = false;

    public void extract()
    {
        //extract floors and walls from prefab
        GameObject grid = prefab.transform.Find("Grid").gameObject;
        GameObject wallGrid = grid.transform.Find("Walls").gameObject;
        GameObject floorGrid = grid.transform.Find("Floors").gameObject;

        Tilemap wallTilemap = wallGrid.GetComponent<Tilemap>();
        Tilemap floorTilemap = floorGrid.GetComponent<Tilemap>();

        List<Vector2Int> tempFloors = GetTilePositions(floorTilemap);
        List<Vector2Int> tempWalls = GetTilePositions(wallTilemap);

        int minX = Mathf.Min(tempFloors.Min(p => p.x), tempWalls.Min(p => p.x));
        int minY = Mathf.Min(tempFloors.Min(p => p.y), tempWalls.Min(p => p.y));

        //normalize 
        floors = tempFloors.Select(t => new Vector2Int(t.x - minX, t.y - minY)).ToList();
        walls = tempWalls.Select(t => new Vector2Int(t.x - minX, t.y - minY)).ToList();

        //remove floor/wall overlap
        floors = floors.Except(walls).ToList();

        //bounds and center
        int maxX = floors.Max(p => p.x);
        int maxY = floors.Max(p => p.y);

        bounds = new BoundsInt(0, 0, 0, maxX + 1, maxY + 1, 1);
        center = new Vector2Int(bounds.size.x / 2, bounds.size.y / 2);

        //extract props
        propPos.Clear();
        propGO.Clear();

        GameObject propGrid = prefab.transform.Find("Props").gameObject;
        Transform gridTransform = prefab.transform.Find("Grid");

        foreach (Transform prop in propGrid.transform)
        {
            // local position relative to the prefab root
            Vector3 local = prop.position - gridTransform.position;

            // convert to integer tile coords
            int x = Mathf.RoundToInt(local.x) - minX;
            int y = Mathf.RoundToInt(local.y) - minY;

            propPos.Add(new Vector2Int(x, y));
            propGO.Add(prop.gameObject);
        }
    }

    public List<Vector2Int> GetTilePositions(Tilemap tilemap)
    {
        //returns list of all tile positions in tilemap
        List<Vector2Int> result = new List<Vector2Int>();

        if (tilemap == null)
            return result;

        BoundsInt b = tilemap.cellBounds;

        foreach (Vector3Int pos in b.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                result.Add(new Vector2Int(pos.x, pos.y));
            }
        }

        return result;
    }

}