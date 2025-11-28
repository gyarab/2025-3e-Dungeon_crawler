using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] int width = 2;

    public HashSet<Vector2Int> GeneratePaths(List<BoundsInt> rooms)
    {
        return GeneratePaths(rooms, width);
    }

    private HashSet<Vector2Int> GeneratePaths(List<BoundsInt> rooms, int width)
    {
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        Dictionary<Vector2Int, BoundsInt> centerToRoom = new Dictionary<Vector2Int, BoundsInt>();
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
        while (roomCenters.Count > 0)
        {
            roomCenters.Remove(currentRoom);
            Vector2Int closestRoom = FindClosestRoom(currentRoom, roomCenters);
            roomCenters.Remove(closestRoom);
            List<Vector2Int> path = CreatePath(currentRoom, closestRoom, width);
            corridors.UnionWith(path);
            currentRoom = closestRoom;
        }
        return corridors;
    }

    private List<Vector2Int> CreatePath(Vector2Int start, Vector2Int target, int width)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int pos = start;

        int maxSteps = 10000;
        float noiseScale = 0.15f;
        float turnChance = 0.25f;
        float sidestepChance = 0.15f;

        while (pos != target && maxSteps-- > 0)
        {
            Vector2Int dir = new Vector2Int((int)Mathf.Sign(target.x - pos.x),(int)Mathf.Sign(target.y - pos.y));

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
}
