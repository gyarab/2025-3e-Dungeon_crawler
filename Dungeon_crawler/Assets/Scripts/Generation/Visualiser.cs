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
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap backgroundTilemap;


    //wall visualisation
    public void VisualiseWalls(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            wallTilemap.SetTile((Vector3Int)pos, wallTile);
            floorTilemap.SetTile((Vector3Int)pos, floorTile);
        }
    }

    //path visualisation (+rooms if HashSet not BoundsInt)
    public void VisualisePaths(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            floorTilemap.SetTile((Vector3Int)pos, pathTile);
        }
    }

    //background visualisation
    public void VisualiseBackground(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
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
