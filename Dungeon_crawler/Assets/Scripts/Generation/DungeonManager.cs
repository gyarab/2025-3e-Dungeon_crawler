using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonManager : MonoBehaviour
{
    public RoomData StartRoom { get; private set; }

    [Header("Generation Settings")]
    [SerializeField] private BSP bsp;
    [SerializeField] private CellularAutomata ca;
    [SerializeField] private PathGenerator pathGen;
    [SerializeField] private WallGenerator wallGen;
    [SerializeField] private Visualiser visualiser;
    private List<GameObject> visualRooms = new List<GameObject>();
    private List<GameObject> visualPaths = new List<GameObject>();

    void Awake()
    {
        Generate();
        AssignRooms();

        Debug.Log("dungeon generated");
    }

    public void AssignRooms()
    {
        // ------------- game logic ------------- //

        List<RoomData> allRooms = new List<RoomData>();
        foreach (BoundsInt room in bsp.GetRooms())
        {
            allRooms.Add(new RoomData(room));
        }

        allRooms[0].type = RoomType.Start;
        StartRoom = allRooms[0];

        allRooms[allRooms.Count - 1].type = RoomType.Boss;
    }

    public void Generate()
    {
        // ------------- generation logic ------------- //

        //clear previous
        visualiser.ClearAll();

        //generate rooms
        bsp.GenerateRooms();
        List<BoundsInt> rooms = bsp.GetRooms();
        HashSet<Vector2Int> caveTiles = new HashSet<Vector2Int>();
        foreach (BoundsInt room in rooms)
        {
            caveTiles.UnionWith(ca.GenerateCaves(room));
        }

        BoundsInt floor = bsp.GetFloorSize();


        //generate walls
        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>(caveTiles);

        //generate paths
        HashSet<Vector2Int> corridors = pathGen.GeneratePaths(rooms);
        tiles.UnionWith(corridors);
        HashSet<Vector2Int> walls = wallGen.GenerateWalls(tiles, 3);

        //visualise rooms and paths
        visualiser.VisualisePaths(tiles);
        visualiser.VisualiseWalls(walls);

        //add background padding
        HashSet<Vector2Int> bgPadding = wallGen.GenerateWalls(tiles, 1);
        tiles.UnionWith(bgPadding);

        //find empty tiles (+ padding to hide outer walls)
        HashSet<Vector2Int> empty = new HashSet<Vector2Int>();
        for (int x = floor.min.x - 20; x < floor.max.x + 20; x++)
        {
            for (int y = floor.min.y - 20; y < floor.max.y + 20; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!tiles.Contains(pos))
                {
                    empty.Add(pos);
                }
            }
        }
        //visualise background
        visualiser.VisualiseBackground(empty);
    }
}
