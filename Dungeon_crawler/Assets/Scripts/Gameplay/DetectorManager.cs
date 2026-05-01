using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectorManager : MonoBehaviour
{
    //manages all detectors on layer
    public HashSet<PlayerDetector> detectors = new HashSet<PlayerDetector>();
    [SerializeField] private GameObject detectorPrefab;
    [SerializeField] private TileBase blank;
    public bool playerDetected = false;
    public HashSet<PlayerDetector> detected = new HashSet<PlayerDetector>();

    public void Initialize(List<Room> allRooms)
    {
        foreach (Room room in allRooms)
        {
            if(room.type == RoomType.Start||!room.hasEnemies)
            {
                continue;
            }
            //instantiates a detector in the center of the room, and sets its room to the tilemap of the room, and adds it to the list of detectors
            GameObject detector = Instantiate(detectorPrefab, room.transform);
            detector.transform.position = room.transform.position;
            detector.name = "detector " + room.transform.name;
            PlayerDetector playerDetector = detector.GetComponent<PlayerDetector>();
            playerDetector.manager = this.GetComponent<DetectorManager>();
            playerDetector.room = room.GetComponentInChildren<Tilemap>();

            foreach (Vector2Int pos in room.floors)
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