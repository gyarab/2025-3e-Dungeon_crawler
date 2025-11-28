using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject character;
    public GameObject playerInstance;

    public DungeonManager dungeonManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (dungeonManager.StartRoom == null)
        {
            Debug.LogError("startroom is null");
            return;
        }
        Vector2 spawnPos = dungeonManager.StartRoom.center;
        //Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        playerInstance = Instantiate(character, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        Debug.Log("player spawned at: " + spawnPos);
        Camera.main.GetComponent<CameraFollow>().target = playerInstance.transform;
    }
}
