using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    //manages the spawning of enemy waves in a room, and the opening and closing of doors
    public bool finished = false;
    public bool activated = false;
    public int totalWaves = 3;
    public int currentWave = 0;
    public int difficulty = 1;
    private PlayerDetector playerDetector;
    private DoorGenerator doorGen;

    public List<List<GameObject>> waves = new List<List<GameObject>>();
    public List<GameObject> activeEnemies = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        difficulty = GameManager.Instance.gameObject.GetComponent<EnemyManager>().globalDifficulty;
        playerDetector = GetComponent<PlayerDetector>();
        playerDetector.onPlayerEnter.AddListener(() => StartCoroutine(HandleWaves()));
    }

    private IEnumerator HandleWaves()
    {
        if (activated)
            yield break;
        activated = true;

        //gets difficulty
        int baseDifficulty = GameManager.Instance.gameObject.GetComponent<EnemyManager>().globalDifficulty;
        int roomDifficulty = baseDifficulty * Mathf.Max(1, transform.parent.GetComponent<Room>().distanceIndex);
        Debug.Log("difficulty: " + roomDifficulty);

        doorGen = GameManager.Instance.dungeonManager.doorGen;
        waves.Clear();
        activeEnemies.Clear();

        for (int i = totalWaves; i > 0; i--)
        {
            //generates waves
            int waveDifficulty = Mathf.Max(1, roomDifficulty / i);
            waves.Add(GameManager.Instance.gameObject.GetComponent<EnemyManager>().GenerateWave(waveDifficulty));
        }

        //close doors
        Debug.Log("Wave started");
        doorGen.CloseDoors();

        for (currentWave = 0; currentWave < totalWaves; currentWave++)
        {
            //spawn wave
            foreach (GameObject enemy in waves[currentWave])
            {
                //instantiates all enemies, waits for them to be killed
                GameObject enemyInstance = Instantiate(enemy, transform.position, Quaternion.identity);

                var accessableFloors = transform.parent.GetComponent<Room>().accessableFloors;
                var floorList = new List<Vector2Int>(accessableFloors);
                if (floorList.Count > 0)
                {
                    Vector2Int randomFloor = floorList[Random.Range(0, floorList.Count)];
                    enemyInstance.transform.position = new Vector3(randomFloor.x, randomFloor.y, 0);
                    enemyInstance.GetComponent<Burrow>().InstaBurry();
                    enemyInstance.GetComponent<Burrow>().Unbury();
                }

                //enemies kept in activeEnemies list to check if all enemies are dead
                activeEnemies.Add(enemyInstance);
                enemyInstance.transform.parent = transform.parent;
            }
            //wait till no enemies left
            yield return new WaitUntil(() => activeEnemies.TrueForAll(item => item == null));
            activeEnemies.Clear();
        }

        //end wave
        finished = true;
        doorGen.OpenDoors();
        Destroy(this);
    }


}
