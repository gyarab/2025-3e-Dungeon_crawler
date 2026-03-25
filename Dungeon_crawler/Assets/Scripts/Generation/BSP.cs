using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PrefabRoom
{
    public PrefabRoomSO prefabRoomSO;
    public RoomType roomType;
    public int minCount;
    public int maxCount;
    public int minPercentDistance;
    public int maxPercentDistance;
}

public class BSP : MonoBehaviour
{
    [Header("Rooms settings")]
    [SerializeField] private BoundsInt floorSize;
    [SerializeField] private int minRoomSize = 10;
    [SerializeField] private int maxRoomSize = 15;
    [SerializeField] private int margin = 1;
    [SerializeField] private bool shift = true;
    [SerializeField] private int maximumRooms = 7;
    [SerializeField] private int minimumRooms = 5;
    [Header("Prefab room settings")]
    [SerializeField] private List<PrefabRoom> prefabRooms = new List<PrefabRoom>();

    private List<BoundsInt> rooms = new List<BoundsInt>();
    private Dictionary<BoundsInt, RoomType> assignedTypes = new Dictionary<BoundsInt, RoomType>();
    private Dictionary<BoundsInt, PrefabRoom> prefabAssignments = new Dictionary<BoundsInt, PrefabRoom>();

    public BoundsInt GetFloorSize()
    {
        return floorSize;
    }
    public List<BoundsInt> GetRooms()
    {
        return rooms;
    }

    public Dictionary<BoundsInt, RoomType> GetAssignedTypes()
    {
        return assignedTypes;
    }

    public Dictionary<BoundsInt, PrefabRoom> GetAssignedPrefabs()
    {
        return prefabAssignments;
    }

    public List<BoundsInt> GetPrefabRooms()
    {
        List<BoundsInt> prefabRoomsBounds = new List<BoundsInt>();
        foreach (PrefabRoom prefabRoom in prefabRooms)
        {
            prefabRoomsBounds.Add(prefabRoom.prefabRoomSO.bounds);
        }
        return prefabRoomsBounds;
    }

    public List<PrefabRoom> GetPrefabRoomsClass()
    {
        return prefabRooms;
    }

    public HashSet<Vector2Int> GetRoomTiles()
    {
        HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
        foreach (BoundsInt room in rooms)
        {
            for (int x = room.min.x; x < room.max.x; x++)
            {
                for (int y = room.min.y; y < room.max.y; y++)
                {
                    roomTiles.Add(new Vector2Int(x, y));
                }
            }
        }
        return roomTiles;
    }

    public List<BoundsInt> GenerateRooms()
    {
        List<BoundsInt> result = null;
        int attempts = 0;

        while (result == null && attempts < 20)
        {
            attempts++;
            result = GenerateRooms(floorSize, minRoomSize, maxRoomSize, margin, minimumRooms, maximumRooms, shift);
        }

        if (result != null)
        {
            AssignPrefabRooms(result);
        }

        return result;
    }

    private List<BoundsInt> GenerateRooms(BoundsInt floor, int MinRoomSize, int MaxRoomSize, int Margin, int minimumRooms, int maximumRooms, bool Shift)
    {
        int safetyCap = 10000;
        int safetyCounter = 0;
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(floor);
        while (roomsQueue.Count > 0 && safetyCounter < safetyCap)
        {
            safetyCounter++;
            BoundsInt room = roomsQueue.Dequeue();
            List<BoundsInt> splitRooms = new List<BoundsInt>();
            if (Random.value > 0.5f)
            {
                splitRooms = SplitY(room, MinRoomSize, MaxRoomSize);
            }
            else
            {
                splitRooms = SplitX(room, MinRoomSize, MaxRoomSize);
            }

            foreach (BoundsInt splitRoom in splitRooms)
            {
                if (splitRoom.size.x > MaxRoomSize || splitRoom.size.y > MaxRoomSize)
                {
                    roomsQueue.Enqueue(splitRoom);
                }
                else
                {
                    roomsList.Add(splitRoom);
                }
            }
        }

        //clamp to min/max rooms
        if (roomsList.Count < minimumRooms)
        {
            return null;
        }
        for (int i = roomsList.Count; i > maximumRooms; i--)
        {
            int rndIndex = Random.Range(0, roomsList.Count);
            roomsList.RemoveAt(rndIndex);
        }
        //shift
        for (int i = 0; i < roomsList.Count; i++)
        {
            int shiftX = Shift ? Random.Range(-Margin, Margin + 1) : 0;
            int shiftY = Shift ? Random.Range(-Margin, Margin + 1) : 0;

            BoundsInt room = roomsList[i];

            room.min += new Vector3Int(Margin + shiftX, Margin + shiftY, 0);
            room.size -= new Vector3Int(Margin * 2 - 1, Margin * 2 - 1, 0);

            roomsList[i] = room;
        }

        rooms = roomsList;
        return roomsList;
    }

    private List<BoundsInt> SplitY(BoundsInt room, int MinRoomSize, int MaxRoomSize)
    {
        if (room.size.y <= MaxRoomSize && room.size.x <= MaxRoomSize)
            return new List<BoundsInt>() { room };
        if (room.size.y < 2 * MinRoomSize - 1)
        {
            if (room.size.x < 2 * MinRoomSize - 1)
                return new List<BoundsInt>() { room };
            else
                return SplitX(room, MinRoomSize, MaxRoomSize);
        }
        List<BoundsInt> rooms = new List<BoundsInt>();

        BoundsInt room1 = new BoundsInt();
        BoundsInt room2 = new BoundsInt();
        int maxSplit = room.size.y - MinRoomSize;
        if (maxSplit <= MinRoomSize) maxSplit = MinRoomSize + 1;
        int rndSplit = Random.Range(MinRoomSize, maxSplit);
        room1.min = room.min;
        room1.size = new Vector3Int(room.size.x, rndSplit, 0);
        room2.min = new Vector3Int(room.min.x, room.min.y + rndSplit, 0);
        room2.size = new Vector3Int(room.size.x, room.size.y - rndSplit, 0);
        rooms.Add(room1);
        rooms.Add(room2);

        return rooms;
    }

    private List<BoundsInt> SplitX(BoundsInt room, int MinRoomSize, int MaxRoomSize)
    {
        if (room.size.y <= MaxRoomSize && room.size.x <= MaxRoomSize)
            return new List<BoundsInt>() { room };
        if (room.size.x < 2 * MinRoomSize - 1)
        {
            if (room.size.y < 2 * MinRoomSize - 1)
                return new List<BoundsInt>() { room };
            else
                return SplitY(room, MinRoomSize, MaxRoomSize);
        }
        List<BoundsInt> rooms = new List<BoundsInt>();

        BoundsInt room1 = new BoundsInt();
        BoundsInt room2 = new BoundsInt();
        int maxSplit = room.size.x - MinRoomSize;
        if (maxSplit <= MinRoomSize) maxSplit = MinRoomSize + 1;
        int rndSplit = Random.Range(MinRoomSize, maxSplit);
        room1.min = room.min;
        room1.size = new Vector3Int(rndSplit, room.size.y, 0);
        room2.min = new Vector3Int(room.min.x + rndSplit, room.min.y, 0);
        room2.size = new Vector3Int(room.size.x - rndSplit, room.size.y, 0);
        rooms.Add(room1);
        rooms.Add(room2);

        return rooms;
    }

    private void AssignPrefabRooms(List<BoundsInt> rooms)
    {
        List<BoundsInt> pair = GetFurthest(rooms);
        BoundsInt spawn = pair[0];
        BoundsInt boss = pair[1];

        rooms.Sort((a, b) =>
        {
            float da = Vector2.Distance((Vector2)spawn.center, (Vector2)a.center);
            float db = Vector2.Distance((Vector2)spawn.center, (Vector2)b.center);
            return da.CompareTo(db);
        });

        Dictionary<BoundsInt, RoomType> finalAssigned = new Dictionary<BoundsInt, RoomType>();
        finalAssigned.Add(spawn, RoomType.Start);
        finalAssigned.Add(boss, RoomType.Boss);

        float maxDist = Vector2.Distance((Vector2)spawn.center, (Vector2)boss.center);

        foreach (PrefabRoom pr in prefabRooms)
        {
            int count = Random.Range(pr.minCount, pr.maxCount + 1);

            for (int i = 0; i < rooms.Count; i++)
            {
                float d = Vector2.Distance((Vector2)spawn.center, (Vector2)rooms[i].center);
                float percent = (d / maxDist) * 100f;

                if (percent >= pr.minPercentDistance && percent <= pr.maxPercentDistance)
                {
                    if (!prefabAssignments.ContainsKey(rooms[i]))
                    {
                        prefabAssignments.Add(rooms[i], pr);
                        if (!finalAssigned.ContainsKey(rooms[i]))
                            finalAssigned.Add(rooms[i], pr.roomType);
                        count--;
                    }

                    if (count <= 0)
                        break;
                }
            }
        }

        assignedTypes = finalAssigned;
        foreach (BoundsInt r in finalAssigned.Keys)
        {
            rooms.Remove(r);
        }
    }

    private List<BoundsInt> GetFurthest(List<BoundsInt> rooms)
    {
        BoundsInt a = rooms[0];
        BoundsInt b = rooms[0];

        float maxDist = -1f;

        foreach (BoundsInt r1 in rooms)
        {
            foreach (BoundsInt r2 in rooms)
            {
                if (r1.Equals(r2)) continue;

                float dist = Vector2.Distance(
                    (Vector2)r1.center,
                    (Vector2)r2.center
                );

                if (dist > maxDist)
                {
                    maxDist = dist;
                    a = r1;
                    b = r2;
                }
            }
        }

        return new List<BoundsInt>() { a, b };
    }

}