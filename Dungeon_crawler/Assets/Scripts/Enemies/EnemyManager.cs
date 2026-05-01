using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //manages the entire layer
    public List<GameObject> enemies;
    public int globalDifficulty = 1;

    public List<GameObject> GenerateWave(int difficulty){
        //generates a wave of random enemies based on difficulty
        //difficulty = tokens left to spend on enemies, each enemy has a token cost equal to its difficulty
        List<GameObject> generatedEnemies = new List<GameObject>();
        int tokensLeft = difficulty;
        while (tokensLeft > 0)
        {
            GameObject enemy = enemies[Random.Range(0, enemies.Count)];
            generatedEnemies.Add(enemy);
            tokensLeft -= enemy.GetComponent<Enemy>().difficulty;
        }
        return generatedEnemies;
    }
}
