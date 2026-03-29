using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] int width = 2;
    private LinkedList<Room> lastRooms = new LinkedList<Room>();
    public List<Room> allRooms = new List<Room>();
    private List<Room> roomsToGo = new List<Room>();

    public HashSet<Vector2Int> GeneratePaths(List<Room> rooms)
    {
        return GeneratePaths(rooms, width);
    }

    private HashSet<Vector2Int> GeneratePaths(List<Room> rooms, int width)
    {
        lastRooms.Clear();
        roomsToGo.Clear();
        allRooms.Clear();

        allRooms = rooms;

        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();

        roomsToGo = new List<Room>(allRooms);
        Room currentRoom = allRooms[0];

        while (roomsToGo.Count > 0)
        {
            
            Room closestRoom = FindClosestRoom(currentRoom, roomsToGo);

            if (currentRoom.type == RoomType.Boss)
            {
                roomsToGo.Remove(currentRoom);
                if (lastRooms.Count == 0)
                    break;
                currentRoom = lastRooms.OrderBy(r => Vector2Int.Distance(r.center, closestRoom.center)).First();
                continue;
            }

            List<Vector2Int> path = CreatePath(currentRoom, closestRoom, width);

            if (path == null)
            {
                if (lastRooms.Count == 0)
                    break;

                roomsToGo.Add(closestRoom);

                currentRoom = lastRooms.Last.Value;
                lastRooms.RemoveLast();
            }
            else
            {
                corridors.UnionWith(path);
                lastRooms.AddLast(currentRoom);
                roomsToGo.Remove(closestRoom);
                currentRoom = closestRoom;
            }
        }

        return corridors;
    }

    private List<Vector2Int> CreatePath(Room start, Room target, int width)
    {
        HashSet<Vector2Int> room1 = start.floors;
        HashSet<Vector2Int> room2 = target.floors;


        HashSet<Vector2Int> trigger = new HashSet<Vector2Int>();
        foreach (Room room in allRooms)
        {
            if (room.floors != null)
                trigger.UnionWith(room.floors);
        }
        trigger.ExceptWith(room1);
        trigger.ExceptWith(room2);

        List<Vector2Int> path = new List<Vector2Int>();


        int maxSteps = 10000;
        float noiseScale = 0.15f;
        float turnChance = 0.25f;
        float sidestepChance = 0.15f;

        Vector2Int pos = start.center;
        Vector2Int targetPos=target.center;

        //find closest access point for prefabs
        if (target.type != RoomType.Generated)
        {
            targetPos = room2.OrderBy(t => Vector2Int.Distance(t, pos)).First();
        }
        if (start.type != RoomType.Generated)
        {
            pos = room1.OrderBy(t => Vector2Int.Distance(t, targetPos)).First();
        }

        while (pos != targetPos && maxSteps-- > 0)
        {
            Vector2Int dir = new Vector2Int((int)Mathf.Sign(targetPos.x - pos.x), (int)Mathf.Sign(targetPos.y - pos.y));

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

        if (start != target)
        {
            if (!start.neighbours.Contains(target))
                start.neighbours.Add(target);

            if (!target.neighbours.Contains(start))
                target.neighbours.Add(start);
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

    private Room FindClosestRoom(Room currentRoom, List<Room> rooms)
    {
        Room closestRoom = rooms[0];
        float closestDistance = Vector2Int.Distance(currentRoom.center, closestRoom.center);
        foreach (Room room in rooms)
        {
            if (room == currentRoom) continue;
            float distance = Vector2Int.Distance(currentRoom.center, room.center);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRoom = room;
            }
        }
        return closestRoom;
    }

    //-----------------DOORS-----------------//

    public HashSet<Vector2Int> GetRoomRing(HashSet<Vector2Int> roomTiles)
    {
        HashSet<Vector2Int> room = new HashSet<Vector2Int>(roomTiles);

        Vector2Int[] dirs = new Vector2Int[]
        {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
        };

        //first ring
        HashSet<Vector2Int> ring1 = new HashSet<Vector2Int>();

        foreach (Vector2Int tile in room)
        {
            foreach (Vector2Int d in dirs)
            {
                Vector2Int n = tile + d;
                if (!room.Contains(n))
                    ring1.Add(n);
            }
        }

        //second ring
        HashSet<Vector2Int> ring2 = new HashSet<Vector2Int>();

        foreach (Vector2Int tile in ring1)
        {
            foreach (Vector2Int d in dirs)
            {
                Vector2Int n = tile + d;

                if (room.Contains(n)) continue;

                if (ring1.Contains(n)) continue;

                ring2.Add(n);
            }
        }

        return ring2;
    }

    public HashSet<Vector2Int> IntersectDoors(Room room, HashSet<Vector2Int> pathTiles)
    {
        HashSet<Vector2Int> ring = GetRoomRing(room.floors);
        HashSet<Vector2Int> doors = new HashSet<Vector2Int>();

        foreach (Vector2Int path in pathTiles)
        {
            if (ring.Contains(path))
                doors.Add(path);
        }

        return doors;
    }
}