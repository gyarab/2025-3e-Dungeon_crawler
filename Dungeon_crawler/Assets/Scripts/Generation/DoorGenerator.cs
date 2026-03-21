using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject doorPrefab;
    [HideInInspector] public HashSet<GameObject> doors = new HashSet<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenDoors();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateDoors(HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            //doorTilemap.SetTile((Vector3Int)pos, doorTile);
            GameObject door = Instantiate(doorPrefab, new Vector3(pos.x+0.5f, pos.y+0.5f, 0), Quaternion.identity);
            door.transform.parent = transform;
            doors.Add(door);
        }
    }

    public void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            //play open animation
            door.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            //play open animation
            door.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
