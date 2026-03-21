using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectorManager : MonoBehaviour
{
    public HashSet<PlayerDetector> detectors = new HashSet<PlayerDetector>();
    [SerializeField] private GameObject detectorPrefab;
    [SerializeField] private TileBase blank;
    public bool playerDetected = false;
    public HashSet<PlayerDetector> detected = new HashSet<PlayerDetector>();

    public void Initialize(List<Room> allRooms)
    {
        foreach (Room room in allRooms)
        {
            if(room.type == RoomType.Start)
            {
                continue;
            }
            GameObject detector = Instantiate(detectorPrefab, room.transform);
            detector.transform.position = room.transform.position;
            detector.name = "detector " + room.transform.name;
            PlayerDetector playerDetector = detector.GetComponent<PlayerDetector>();
            playerDetector.manager = this.GetComponent<DetectorManager>();

            HashSet<Vector2Int> shrunk = new HashSet<Vector2Int>();
            foreach (Vector2Int pos in room.floors)
            {
                if (room.floors.Contains(new Vector2Int(pos.x + 1, pos.y)) &&
                    room.floors.Contains(new Vector2Int(pos.x - 1, pos.y)) &&
                    room.floors.Contains(new Vector2Int(pos.x, pos.y + 1)) &&
                    room.floors.Contains(new Vector2Int(pos.x, pos.y - 1)))
                {
                    shrunk.Add(pos);
                }
            }

            foreach (Vector2Int pos in shrunk)
            {
                Vector3 worldPos = new Vector3(pos.x, pos.y, 0);
                Vector3Int localPos = playerDetector.room.WorldToCell(worldPos);
                playerDetector.room.SetTile(localPos, blank);
            }
        }
    }

    void Update()
    {
        playerDetected = detected.Count > 0;
    }
}