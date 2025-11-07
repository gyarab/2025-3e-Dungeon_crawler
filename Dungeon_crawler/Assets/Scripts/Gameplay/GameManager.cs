using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject character;

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
        Debug.Log("player spawned at: " + spawnPos);
    }
}
