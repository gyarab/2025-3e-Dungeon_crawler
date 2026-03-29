using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.LightTransport;

public class DungeonManager : MonoBehaviour
{
    public Room StartRoom { get; private set; }

    [Header("Managers")]
    public DetectorManager detectorMan;
    [Header("Generation references")]
    public BSP bsp;
    public CellularAutomata ca;
    public FloodFill ff;
    public PathGenerator pathGen;
    public WallGenerator wallGen;
    public WallIslandGenerator wallIslandGen;
    public Visualiser visualiser;
    public DoorGenerator doorGen;
    [Header("Props references")]
    public PropManager propMan;
    public List<Room> allRooms = new List<Room>();
    public List<Room> randomRooms = new List<Room>();
    public List<Room> prefabRooms = new List<Room>();

    //room floors
    HashSet<Vector2Int> allFloorTiles = new HashSet<Vector2Int>();
    HashSet<Vector2Int> prefabFloorTiles = new HashSet<Vector2Int>();
    HashSet<Vector2Int> genFloorTiles = new HashSet<Vector2Int>();
    //walls
    private HashSet<Vector2Int> allWallTiles = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> genWallTiles = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> prefabWallTiles = new HashSet<Vector2Int>();

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
        allRooms.Clear();
        visualiser.ClearAll();
        propMan.ClearAllProps();
        doorGen.ClearDoors();

        allRooms = bsp.GenerateRooms();

        // ---------------- CA FOR BSP ROOMS ONLY ---------------- //
        
        foreach (Room room in allRooms.Where(r => r.type == RoomType.Generated))
        {
            HashSet<Vector2Int> tiles = ca.GenerateCaves(room.bounds);
            room.floors = tiles;
            genFloorTiles.UnionWith(tiles);
        }
        

        // ---------------- PREFAB FLOORS MERGED ---------------- //

        prefabFloorTiles = new HashSet<Vector2Int>();

        foreach (Room room in allRooms.Where(r => r.type != RoomType.Generated))
        {
            if (room.floors == null || room.walls==null) continue;
            if(room.type == RoomType.Start)
                StartRoom = room;

            Vector2Int bspCenter = room.center;
            Vector2Int prefabCenter = room.prefabRoomSO.center;

            Vector2Int centerOffset = bspCenter - prefabCenter;

            HashSet<Vector2Int> worldFloors = new HashSet<Vector2Int>();
            foreach (Vector2Int local in room.floors)
            {
                Vector2Int world = local + centerOffset;
                prefabFloorTiles.Add(world);
                worldFloors.Add(world);
            }
            room.floors = worldFloors;
            HashSet<Vector2Int> worldWalls = new HashSet<Vector2Int>();
            foreach (Vector2Int local in room.walls)
            {
                Vector2Int world = local + centerOffset;
                prefabWallTiles.Add(world);
                worldWalls.Add(world);
            }
            //Debug.Log(room.transform.name + ":    walls: " + worldWalls.Count() + "    ; floors: " + worldFloors.Count());
            room.walls = worldWalls;

        }

        //merge all floor tiles for path gen
        allFloorTiles.UnionWith(prefabFloorTiles);
        allFloorTiles.UnionWith(genFloorTiles);

        BoundsInt floor = bsp.GetFloorSize();

        //generate paths
        HashSet<Vector2Int> pathTiles = pathGen.GeneratePaths(allRooms);

        allFloorTiles.UnionWith(pathTiles);

        //generate doors
        HashSet<Vector2Int> doorTiles = new HashSet<Vector2Int>();

        foreach (Room room in allRooms)
        {
            HashSet<Vector2Int> roomDoors = pathGen.IntersectDoors(room, pathTiles);
            doorTiles.UnionWith(roomDoors);
        }

        doorGen.GenerateDoors(doorTiles);

        //true = pref; false = gen
        Dictionary<Vector2Int, bool> tempFloorTiles = new Dictionary<Vector2Int, bool>();
        foreach (Vector2Int tile in allFloorTiles)
            tempFloorTiles.Add(tile, false);
        foreach (Vector2Int tile in pathTiles)
            tempFloorTiles[tile] = false;
        foreach (Vector2Int tile in prefabFloorTiles)
            tempFloorTiles[tile] = true;

        List<HashSet<Vector2Int>> walls = wallGen.GenerateWalls(tempFloorTiles, 3);

        genWallTiles = walls[0]; 
        prefabWallTiles = walls[1];
        allWallTiles.UnionWith(genWallTiles.Union(prefabWallTiles).ToHashSet());

        HashSet<Vector2Int> viableIslandFloors = new HashSet<Vector2Int>(allFloorTiles.Except(prefabFloorTiles));

        //create wall islands and smooth caves
        HashSet<Vector2Int> wallIslands = wallIslandGen.GenerateWallIslands(viableIslandFloors, pathTiles);
        allWallTiles.UnionWith(wallIslands);

        HashSet<Vector2Int> genWallsOnly = new HashSet<Vector2Int>(genWallTiles.Union(wallIslands));   // only generated walls

        HashSet<Vector2Int> genFloorsOnly = new HashSet<Vector2Int>(genFloorTiles.Union(pathTiles));    // only generated floors + paths

        ca.SmoothCaves(genWallsOnly, genFloorsOnly);


        //visualise floor and walls before padding to check for errors
        visualiser.VisualisePaths(allFloorTiles);
        visualiser.VisualiseWalls(allWallTiles);

        //generate wall padding for bg
        List<HashSet<Vector2Int>> paddings = wallGen.GenerateWalls(tempFloorTiles, 1);
        HashSet<Vector2Int> bgPadding = new HashSet<Vector2Int>();
        foreach(HashSet<Vector2Int> padding in paddings)
            bgPadding.UnionWith(padding);
        allFloorTiles.UnionWith(bgPadding);

        //get empty tiles to visualise bg
        HashSet<Vector2Int> empty = new HashSet<Vector2Int>();
        for (int x = floor.min.x - 20; x < floor.max.x + 20; x++)
            for (int y = floor.min.y - 20; y < floor.max.y + 20; y++)
                if (!allFloorTiles.Contains(new Vector2Int(x, y)))
                    empty.Add(new Vector2Int(x, y));

        //visualise bg
        visualiser.VisualiseBackground(empty);

        // ----------------- PROPS ---------------- //

        HashSet<Vector2Int> propFloorTiles = new HashSet<Vector2Int>(allFloorTiles);

        propFloorTiles.ExceptWith(doorTiles);
        foreach(Room room in allRooms.Where(r => r.type != RoomType.Generated))
        {
            propMan.GenerateSetProps(room.props, room);
        }

        propMan.StartCoroutine(propMan.GenerateAllProps(propFloorTiles, allWallTiles));

        // ----------------- SPECIALS ---------------- //

        ff.FindAccessibleTiles(allRooms,allFloorTiles);

        /*
        foreach (Room room in allRooms)
        {
            Debug.Log(room.name);
            foreach(Room neighbour in room.neighbours)
            {
                Debug.Log("  - " + neighbour.name);
            }
        }
        */
    }
}