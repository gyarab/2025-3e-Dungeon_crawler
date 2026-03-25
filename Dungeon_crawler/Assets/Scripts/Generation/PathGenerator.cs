using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] int width = 2;
    private LinkedList<Vector2Int> lastRooms = new LinkedList<Vector2Int>();
    public Dictionary<Vector2Int, HashSet<Vector2Int>> roomFloors = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    public List<BoundsInt> rooms = new List<BoundsInt>();
    Dictionary<Vector2Int, BoundsInt> centerToRoom = new Dictionary<Vector2Int, BoundsInt>();
    List<Vector2Int> roomCenters = new List<Vector2Int>();

    public HashSet<Vector2Int> GeneratePaths(List<BoundsInt> rooms)
    {
        return GeneratePaths(rooms, width);
    }

    private HashSet<Vector2Int> GeneratePaths(List<BoundsInt> rooms, int width)
    {
        centerToRoom.Clear();
        this.rooms.Clear();
        lastRooms.Clear();
        roomCenters.Clear();

        this.rooms = rooms;

        foreach (BoundsInt room in rooms)
        {
            Vector2Int center = new Vector2Int(
                Mathf.FloorToInt(room.center.x),
                Mathf.FloorToInt(room.center.y)
            );
            roomCenters.Add(center);
            centerToRoom.Add(center, room);
        }

        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        Vector2Int currentRoom = roomCenters[0];

        foreach(Vector2Int room in roomCenters)
        {
            Debug.Log("pg: "+room);
        }

        while (roomCenters.Count > 0)
        {
            Vector2Int closestRoom = FindClosestRoom(currentRoom, roomCenters);

            List<Vector2Int> path = CreatePath(currentRoom, closestRoom, width);

            if (path == null)
            {
                if (lastRooms.Count == 0)
                    break;

                roomCenters.Add(closestRoom);

                currentRoom = lastRooms.Last.Value;
                lastRooms.RemoveLast();
            }
            else
            {
                corridors.UnionWith(path);
                lastRooms.AddLast(currentRoom);
                roomCenters.Remove(closestRoom);
                currentRoom = closestRoom;
            }
        }

        return corridors;
    }

    private HashSet<Vector2Int> GetRoomFloors(Vector2Int center, List<BoundsInt> rooms)
    {
        if (roomFloors.ContainsKey(center))
            return roomFloors[center];

        throw new Exception("center not in any room");
    }

    private List<Vector2Int> CreatePath(Vector2Int start, Vector2Int target, int width)
    {
        //DONT CREATE PATH FROM BOSS ROOM

        HashSet<Vector2Int> room1 = GetRoomFloors(start, rooms);
        HashSet<Vector2Int> room2 = GetRoomFloors(target, rooms);

        HashSet<Vector2Int> trigger = new HashSet<Vector2Int>();
        foreach (HashSet<Vector2Int> floors in roomFloors.Values)
        {
            if (floors != null)
                trigger.UnionWith(floors);
        }
        trigger.ExceptWith(room1);
        trigger.ExceptWith(room2);

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int pos = start;

        int maxSteps = 10000;
        float noiseScale = 0.15f;
        float turnChance = 0.25f;
        float sidestepChance = 0.15f;

        while (pos != target && maxSteps-- > 0)
        {
            Vector2Int dir = new Vector2Int((int)Mathf.Sign(target.x - pos.x), (int)Mathf.Sign(target.y - pos.y));

            float n = Mathf.PerlinNoise(pos.x * noiseScale, pos.y * noiseScale);

            if (n > 0.6f || Random.value < turnChance)
            {
                dir = RandomDirection();
            }
            else if (Random.value < sidestepChance)
            {
                dir = new Vector2Int(dir.y, -dir.x);
            }

            pos += dir;

            int corridorWidth = width + Random.Range(-1, 1);
            corridorWidth = Mathf.Clamp(corridorWidth, 1, width);

            for (int dx = -corridorWidth; dx <= corridorWidth; dx++)
            {
                for (int dy = -corridorWidth; dy <= corridorWidth; dy++)
                {
                    if (dx * dx + dy * dy <= corridorWidth * corridorWidth)
                    {
                        if (trigger.Contains(new Vector2Int(pos.x + dx, pos.y + dy)))
                        {
                            return null;
                        }
                        path.Add(new Vector2Int(pos.x + dx, pos.y + dy));
                    }
                }
            }
        }

        return path;
    }

    private Vector2Int RandomDirection()
    {
        Vector2Int[] dirs =
        {
        new Vector2Int(1,0), new Vector2Int(-1,0),
        new Vector2Int(0,1), new Vector2Int(0,-1),
        new Vector2Int(1,1), new Vector2Int(-1,1),
        new Vector2Int(1,-1), new Vector2Int(-1,-1)
        };
        return dirs[Random.Range(0, dirs.Length)];
    }

    private Vector2Int FindClosestRoom(Vector2Int currentRoom, List<Vector2Int> roomCenters)
    {
        Vector2Int closestRoom = roomCenters[0];
        float closestDistance = Vector2Int.Distance(currentRoom, closestRoom);
        foreach (Vector2Int room in roomCenters)
        {
            float distance = Vector2Int.Distance(currentRoom, room);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRoom = room;
            }
        }
        return closestRoom;
    }

    //doors
    public HashSet<Vector2Int> GetRoomPadding(BoundsInt room)
    {
        HashSet<Vector2Int> padding = new HashSet<Vector2Int>();

        // expanded bounds
        BoundsInt expanded = new BoundsInt(
            room.xMin - 1,
            room.yMin - 1,
            0,
            room.size.x + 2,
            room.size.y + 2,
            1
        );

        // original room tiles
        HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
        for (int x = room.xMin; x < room.xMax; x++)
            for (int y = room.yMin; y < room.yMax; y++)
                roomTiles.Add(new Vector2Int(x, y));

        // padding = expanded minus original
        for (int x = expanded.xMin; x < expanded.xMax; x++)
        {
            for (int y = expanded.yMin; y < expanded.yMax; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!roomTiles.Contains(pos))
                    padding.Add(pos);
            }
        }

        return padding;
    }

    public HashSet<Vector2Int> IntersectDoors(
    BoundsInt room,
    HashSet<Vector2Int> pathTiles)
    {
        HashSet<Vector2Int> padding = GetRoomPadding(room);
        HashSet<Vector2Int> doors = new HashSet<Vector2Int>();

        foreach (Vector2Int path in pathTiles)
        {
            if (padding.Contains(path))
                doors.Add(path);
        }

        return doors;
    }
}