using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<Room> allRooms = new List<Room>();

    public BoundsInt GetFloorSize()
    {
        return floorSize;
    }

    public List<Room> GenerateRooms()
    {
        List<Room> result = null;
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

    private List<Room> GenerateRooms(BoundsInt floor, int MinRoomSize, int MaxRoomSize, int Margin, int minimumRooms, int maximumRooms, bool Shift)
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

        int index = 1;
        foreach (BoundsInt room in roomsList)
        {
            GameObject roomGO = new GameObject("Room "+index);
            roomGO.transform.position = new Vector3Int((int)Mathf.Ceil(room.center.x), (int)Mathf.Ceil(room.center.y),0);
            Room roomSC = roomGO.AddComponent<Room>();  
            roomSC.bounds = room;
            roomSC.center = new Vector2Int(Mathf.FloorToInt(room.center.x), Mathf.FloorToInt(room.center.y));
            allRooms.Add(roomSC);
            index++;
        }
        return allRooms;
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

    private void AssignPrefabRooms(List<Room> rooms)
    {
        List<Room> pair = GetFurthest(rooms);
        Room spawn = pair[0];
        Room boss = pair[1];

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].center == spawn.center)
            {
                rooms[i].type = RoomType.Start;
            }
            else if (rooms[i].center == boss.center)
            {
                rooms[i].type = RoomType.Boss;
            }
        }

        rooms.Sort((a, b) =>
        {
            float da = Vector2.Distance((Vector2)spawn.center, (Vector2)a.center);
            float db = Vector2.Distance((Vector2)spawn.center, (Vector2)b.center);
            return da.CompareTo(db);
        });

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
                    for (int j = 0; j < allRooms.Count; j++)
                    {
                        if (allRooms[j].center == rooms[i].center)
                        {
                            Room room = allRooms[j];
                            room.type = pr.roomType;
                            room.prefabRoomSO = pr.prefabRoomSO;

                            room.floors = new HashSet<Vector2Int>(pr.prefabRoomSO.floors);
                            room.walls = new HashSet<Vector2Int>(pr.prefabRoomSO.walls);


                            room.props = new Dictionary<Vector2Int, GameObject>();

                            for (int p = 0; p < pr.prefabRoomSO.propPos.Count; p++)
                            {
                                Dictionary<Vector2Int, GameObject> dict = new Dictionary<Vector2Int, GameObject>();
                                room.props.Add(pr.prefabRoomSO.propPos[p], pr.prefabRoomSO.propGO[p]);
                            }

                            room.transform.name += " (" + pr.roomType.ToString() + ")";
                            room.hasEnemies = pr.prefabRoomSO.hasEnemies;

                            count--;
                            break;
                        }
                    }
                    if (count <= 0)
                        break;
                }
            }
        }
    }

    private List<Room> GetFurthest(List<Room> rooms)
    {
        Room a = rooms[0];
        Room b = rooms[0];

        float maxDist = -1f;

        foreach (Room r1 in rooms)
        {
            foreach (Room r2 in rooms)
            {
                if (r1.Equals(r2)) continue;

                float dist = Vector2.Distance((Vector2)r1.center, (Vector2)r2.center);

                if (dist > maxDist)
                {
                    maxDist = dist;
                    a = r1;
                    b = r2;
                }
            }
        }

        return new List<Room>() { a, b };
    }

}