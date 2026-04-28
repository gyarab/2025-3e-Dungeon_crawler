using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public int globalDifficulty = 1;

    public List<GameObject> GenerateWave(int difficulty){
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
