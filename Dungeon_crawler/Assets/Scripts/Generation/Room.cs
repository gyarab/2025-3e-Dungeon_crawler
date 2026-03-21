using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public BoundsInt bounds;
    public HashSet<Vector2Int> floors = new HashSet<Vector2Int>();
    public Vector2Int center;
    public RoomType type;

    public void Initialize(BoundsInt bounds, HashSet<Vector2Int> floors, RoomType type = RoomType.Normal)
    {
        this.bounds = bounds;
        this.floors = floors;
        this.center = new Vector2Int(Mathf.FloorToInt(bounds.center.x), Mathf.FloorToInt(bounds.center.y));
        this.type = type;
    }
}

public enum RoomType
{
    Normal,
    Start,
    Boss,
    Treasure,
    Shop
}

