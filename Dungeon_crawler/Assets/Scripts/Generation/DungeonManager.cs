using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonManager : MonoBehaviour
{
    public Room StartRoom { get; private set; }

    [Header("Managers")]
    public DetectorManager detectorMan;
    [Header("Generation references")]
    public BSP bsp;
    public CellularAutomata ca;
    public PathGenerator pathGen;
    public WallGenerator wallGen;
    public WallIslandGenerator wallIslandGen;
    public Visualiser visualiser;
    public DoorGenerator doorGen;
    [Header("Props references")]
    public PropManager propMan;
    private List<GameObject> visualRooms = new List<GameObject>();
    private List<GameObject> visualPaths = new List<GameObject>();
    public List<Room> allRooms = new List<Room>();

    void Awake()
    {
        Generate();

        Debug.Log("dungeon generated");

        // ------------------- INITIALIZATION ---------------------- //
        detectorMan.Initialize(allRooms);
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

        int index = 1;
        foreach (BoundsInt room in rooms)
        {
            GameObject roomGO = new GameObject("Room " + index);

            Room roomScript = roomGO.AddComponent<Room>();
            roomScript.Initialize(room, null);
            roomGO.transform.position = Vector3.zero;
            allRooms.Add(roomScript);
            index++;
        }

        allRooms[0].type = RoomType.Start;
        StartRoom = allRooms[0];

        allRooms[allRooms.Count - 1].type = RoomType.Boss;

        foreach (BoundsInt room in rooms)
        {
            HashSet<Vector2Int> tiles = ca.GenerateCaves(room);
            for (int i = 0; i < allRooms.Count; i++)
            {
                if (allRooms[i].bounds == room)
                {
                    allRooms[i].floors = tiles;
                }
            }
            roomTiles.UnionWith(tiles);
        }

        BoundsInt floor = bsp.GetFloorSize();



        //generate walls
        HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>(roomTiles);

        //generate paths
        HashSet<Vector2Int> pathTiles = pathGen.GeneratePaths(rooms);
        floorTiles.UnionWith(pathTiles);

        //generate dioors
        HashSet<Vector2Int> doorTiles = new HashSet<Vector2Int>();

        foreach (BoundsInt room in rooms)
        {
            var roomDoors = pathGen.IntersectDoors(room, pathTiles);
            doorTiles.UnionWith(roomDoors);
        }

        //visualize dioors
        doorGen.GenerateDoors(doorTiles);

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

        HashSet<Vector2Int> propFloorTiles = new HashSet<Vector2Int>(floorTiles);

        //remove door tiles from floor tiles
        propFloorTiles.ExceptWith(doorTiles);

        //generate props
        propMan.GenerateAllProps(propFloorTiles, wallTiles);

    }
}