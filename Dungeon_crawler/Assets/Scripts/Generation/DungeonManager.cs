using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonManager : MonoBehaviour
{
    public RoomData StartRoom { get; private set; }

    [Header("Generation references")]
    [SerializeField] private BSP bsp;
    [SerializeField] private CellularAutomata ca;
    [SerializeField] private PathGenerator pathGen;
    [SerializeField] private WallGenerator wallGen;
    [SerializeField] private WallIslandGenerator wallIslandGen;
    [SerializeField] private Visualiser visualiser;
    [Header("Props references")]
    [SerializeField] private PropManager propMan;
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
        HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
        foreach (BoundsInt room in rooms)
        {
            roomTiles.UnionWith(ca.GenerateCaves(room));
        }

        BoundsInt floor = bsp.GetFloorSize();



        //generate walls
        HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>(roomTiles);

        //generate paths
        HashSet<Vector2Int> pathTiles = pathGen.GeneratePaths(rooms);
        floorTiles.UnionWith(pathTiles);
        HashSet<Vector2Int> wallTiles = wallGen.GenerateWalls(floorTiles, 3);

        //generate cave islands
        HashSet<Vector2Int> wallIslands = wallIslandGen.GenerateWallIslands(roomTiles, pathTiles);
        wallTiles.UnionWith(wallIslands);
        ca.SmoothCaves(wallTiles, floorTiles);

        //visualise floors and walls
        visualiser.VisualisePaths(floorTiles);
        visualiser.VisualiseWalls(wallTiles);


        //add background padding
        HashSet<Vector2Int> bgPadding = wallGen.GenerateWalls(floorTiles, 1);
        floorTiles.UnionWith(bgPadding);

        //find empty tiles (+ padding to hide outer walls)
        HashSet<Vector2Int> empty = new HashSet<Vector2Int>();
        for (int x = floor.min.x - 20; x < floor.max.x + 20; x++)
        {
            for (int y = floor.min.y - 20; y < floor.max.y + 20; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!floorTiles.Contains(pos))
                {
                    empty.Add(pos);
                }
            }
        }
        //visualise background
        visualiser.VisualiseBackground(empty);

        // ------------------- PROPS ---------------------- //

        //generate props
        propMan.GenerateAllProps(floorTiles, wallTiles);

    }
}