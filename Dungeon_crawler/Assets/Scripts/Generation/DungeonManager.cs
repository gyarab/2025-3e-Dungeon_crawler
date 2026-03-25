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
    public List<Room> allRooms = new List<Room>();

    private List<Vector2Int> prefabRoomFloors;
    private List<Vector2Int> prefabRoomWalls;
    private List<Dictionary<Vector2Int, GameObject>> prefabRoomProps;

    private List<PrefabRoom> prefabRoomsClass = new List<PrefabRoom>();

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

        bsp.GenerateRooms();
        List<BoundsInt> rooms = bsp.GetRooms();
        Dictionary<BoundsInt, RoomType> assigned = bsp.GetAssignedTypes();
        Dictionary<BoundsInt, PrefabRoom> prefabAssigned = bsp.GetAssignedPrefabs();

        HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
        prefabRoomsClass = bsp.GetPrefabRoomsClass();

        int index = 1;

        // ---------------- NORMAL ROOMS ---------------- //
        foreach (BoundsInt room in rooms)
        {
            if (assigned.ContainsKey(room)) continue;

            GameObject roomGO = new GameObject("Room " + index);
            Room roomScript = roomGO.AddComponent<Room>();
            roomScript.Initialize(room, null);
            roomGO.transform.position = Vector3.zero;
            allRooms.Add(roomScript);
            index++;
        }

        // ---------------- PREFAB ROOMS ---------------- //
        foreach (var pair in prefabAssigned)
        {
            BoundsInt slot = pair.Key;
            PrefabRoom pr = pair.Value;

            GameObject roomGO = new GameObject("Prefab room " + pr.roomType);
            Room roomScript = roomGO.AddComponent<Room>();

            HashSet<Vector2Int> floors = new HashSet<Vector2Int>();
            foreach (Vector2Int p in pr.prefabRoomSO.floors)
            {
                Vector2Int offset = new Vector2Int(
                    p.x - pr.prefabRoomSO.bounds.min.x + slot.min.x,
                    p.y - pr.prefabRoomSO.bounds.min.y + slot.min.y
                );
                floors.Add(offset);
            }

            roomScript.Initialize(slot, floors);
            roomScript.type = assigned[slot];

            roomGO.transform.position = Vector3.zero;
            allRooms.Add(roomScript);

            if (roomScript.type == RoomType.Start)
                StartRoom = roomScript;

            index++;
        }

        // ---------------- CA FOR BSP ROOMS ONLY ---------------- //
        foreach (BoundsInt room in rooms)
        {
            if (assigned.ContainsKey(room)) continue;

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

        // ---------------- PREFAB FLOORS MERGED ---------------- //
        foreach (var pair in prefabAssigned)
        {
            PrefabRoom pr = pair.Value;
            BoundsInt slot = pair.Key;

            foreach (Vector2Int p in pr.prefabRoomSO.floors)
            {
                Vector2Int offset = new Vector2Int(
                    p.x - pr.prefabRoomSO.bounds.min.x + slot.min.x,
                    p.y - pr.prefabRoomSO.bounds.min.y + slot.min.y
                );
                roomTiles.Add(offset);
            }
        }

        BoundsInt floor = bsp.GetFloorSize();

        HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>(roomTiles);

        pathGen.roomFloors.Clear();
        foreach (Room room in allRooms)
        {
            Vector2Int center = room.center;
            HashSet<Vector2Int> floors = room.floors;

            if (!pathGen.roomFloors.ContainsKey(center))
                pathGen.roomFloors.Add(center, floors);
            else
                pathGen.roomFloors[center] = floors;
        }
        List<BoundsInt> allRoomBounds = new List<BoundsInt>();
        foreach(Room room in allRooms) 
        {
            allRoomBounds.Add(room.bounds);
        }
        HashSet<Vector2Int> pathTiles = pathGen.GeneratePaths(allRoomBounds);
        floorTiles.UnionWith(pathTiles);

        HashSet<Vector2Int> doorTiles = new HashSet<Vector2Int>();

        foreach (BoundsInt room in allRoomBounds)
        {
            var roomDoors = pathGen.IntersectDoors(room, pathTiles);
            doorTiles.UnionWith(roomDoors);
        }

        doorGen.GenerateDoors(doorTiles);

        HashSet<Vector2Int> wallTiles = wallGen.GenerateWalls(floorTiles, 3);

        HashSet<Vector2Int> viableIslandFloors = new HashSet<Vector2Int>(floorTiles);
        foreach (var pair in prefabAssigned)
        {
            PrefabRoom pr = pair.Value;
            foreach (Vector2Int p in pr.prefabRoomSO.floors)
            {
                Vector2Int offset = new Vector2Int(
                    p.x - pr.prefabRoomSO.bounds.min.x + pair.Key.min.x,
                    p.y - pr.prefabRoomSO.bounds.min.y + pair.Key.min.y
                );
                viableIslandFloors.Remove(offset);
            }
        }

        //create wall islands and smooth caves
        HashSet<Vector2Int> wallIslands = wallIslandGen.GenerateWallIslands(
            viableIslandFloors, pathTiles);
        wallTiles.UnionWith(wallIslands);
        ca.SmoothCaves(wallTiles, floorTiles);

        //visualise floor and walls before padding to check for errors
        visualiser.VisualisePaths(floorTiles);
        visualiser.VisualiseWalls(wallTiles);

        //generate wall padding for bg
        HashSet<Vector2Int> bgPadding = wallGen.GenerateWalls(floorTiles, 1);
        floorTiles.UnionWith(bgPadding);

        //get empty tiles to visualise bg
        HashSet<Vector2Int> empty = new HashSet<Vector2Int>();
        for (int x = floor.min.x - 20; x < floor.max.x + 20; x++)
            for (int y = floor.min.y - 20; y < floor.max.y + 20; y++)
                if (!floorTiles.Contains(new Vector2Int(x, y)))
                    empty.Add(new Vector2Int(x, y));

        //visualise bg
        visualiser.VisualiseBackground(empty);

        // ----------------- PROPS ---------------- //

        HashSet<Vector2Int> propFloorTiles = new HashSet<Vector2Int>(floorTiles);

        propFloorTiles.ExceptWith(doorTiles);

        propMan.GenerateAllProps(propFloorTiles, wallTiles);
    }
}