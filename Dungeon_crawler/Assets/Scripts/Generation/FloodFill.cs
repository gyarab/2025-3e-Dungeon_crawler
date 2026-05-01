using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodFill : MonoBehaviour
{
    public void FindAccessibleTiles(List<Room> allRooms, HashSet<Vector2Int> allFloorTiles)
    {

        //tile -> room
        Dictionary<Vector2Int, Room> tileToRoom = new Dictionary<Vector2Int, Room>();

        foreach (Room room in allRooms)
        {
            foreach (Vector2Int t in room.floors)
            {
                tileToRoom[t] = room;
            }
            room.accessableFloors = new HashSet<Vector2Int>();
        }

        //bfs

        Room startRoom = allRooms.First(r => r.type == RoomType.Start);
        Vector2Int start = startRoom.floors.OrderBy(t => Vector2Int.Distance(t, startRoom.center)).First();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(-1,1),
            new Vector2Int(1,-1), new Vector2Int(-1,-1)
        };

        //distance from start to tile
        Dictionary<Vector2Int, int> tileDistance = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        visited.Add(start);
        tileDistance[start] = 0;

        //flood fill

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int d in dirs)
            {
                Vector2Int next = current + d;

                if (!allFloorTiles.Contains(next))
                    continue;
                if (visited.Contains(next))
                    continue;

                visited.Add(next);
                queue.Enqueue(next);
                tileDistance[next] = tileDistance[current] + 1;
            }
        }

        //assign tiles to rooms

        foreach (Vector2Int wall in GameManager.Instance.dungeonManager.allWallTiles)
        {
            foreach (Vector2Int tile in visited)
            {
                if (tile== wall)
                {
                    visited.Remove(tile);
                    break;
                }
            }
        }

        foreach (Vector2Int tile in visited)
        {
            if(!tileToRoom.ContainsKey(tile)) //path
                continue;
            Room r = tileToRoom[tile];
            r.accessableFloors.Add(tile);
        }


        //room distance 
        Dictionary<Room, int> roomDistances = new Dictionary<Room, int>();

        foreach (Room room in allRooms)
        {
            int minDist = int.MaxValue;

            foreach (Vector2Int tile in room.accessableFloors)
            {
                if (tileDistance.TryGetValue(tile, out int d))
                    minDist = Mathf.Min(minDist, d);
            }

            roomDistances[room] = (minDist == int.MaxValue ? -1 : minDist);
        }

        //sort by distance
        var sorted = roomDistances
            .Where(kvp => kvp.Value >= 0)
            .OrderBy(kvp => kvp.Value)
            .ToList();

        //assign index
        int index = 1;
        foreach (KeyValuePair<Room,int> kvp in sorted)
        {
            kvp.Key.distanceIndex = index;
            index++;
        }
    }
}