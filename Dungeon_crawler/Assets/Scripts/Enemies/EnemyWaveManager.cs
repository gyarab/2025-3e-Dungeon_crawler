using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public bool finished = false;
    private int currentWave = 0;
    private int totalWaves;
    public int difficulty=10;
    private PlayerDetector playerDetector;
    private DoorGenerator doorGen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorGen = GameManager.Instance.dungeonManager.doorGen;
        playerDetector = GetComponent<PlayerDetector>();
        playerDetector.onPlayerEnter.AddListener(onPlayerEnter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onPlayerEnter()
    {
        Debug.Log("Wave started");
        doorGen.CloseDoors();
    }


}
