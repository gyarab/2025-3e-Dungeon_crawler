using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Visualiser : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase pathTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase backgroundTile;

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap backgroundTilemap;



    //room visualisation - BoundsInt
    public void VisualiseRooms(List<BoundsInt> bounds)
    {
        foreach (BoundsInt bound in bounds)
        {
            for (int x = bound.xMin; x < bound.xMax; x++)
            {
                for (int y = bound.yMin; y < bound.yMax; y++)
                {
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }
    }

    //wall visualisation
    public void VisualiseWalls(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions.Distinct())
        {
            wallTilemap.SetTile((Vector3Int)pos, wallTile);
            floorTilemap.SetTile((Vector3Int)pos, floorTile);
        }
    }

    //path visualisation (+rooms if HashSet not BoundsInt)
    public void VisualisePaths(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions.Distinct())
        {
            floorTilemap.SetTile((Vector3Int)pos, pathTile);
        }
    }

    //background visualisation
    public void VisualiseBackground(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions.Distinct())
        {
            backgroundTilemap.SetTile((Vector3Int)pos, backgroundTile);
        }
    }

    //clear
    public void ClearAll()
    {
        floorTilemap?.ClearAllTiles();
        wallTilemap?.ClearAllTiles();
        backgroundTilemap?.ClearAllTiles();
    }
}
