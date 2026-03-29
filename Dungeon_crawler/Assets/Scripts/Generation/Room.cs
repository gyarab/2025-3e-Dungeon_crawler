using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public BoundsInt bounds;
    public HashSet<Vector2Int> floors = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> walls = new HashSet<Vector2Int>(); 
    public Dictionary<Vector2Int, GameObject> props = new Dictionary<Vector2Int, GameObject>();
    public HashSet<Vector2Int> accessableFloors = new HashSet<Vector2Int>();
    public int distanceIndex;
    public Vector2Int center;
    public RoomType type;
    public PrefabRoomSO prefabRoomSO;
    public List<Room> neighbours = new List<Room>();
}

public enum RoomType
{
    Generated,
    Start,
    Boss,
    Treasure,
    Shop
}

