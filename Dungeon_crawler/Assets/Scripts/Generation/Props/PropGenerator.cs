using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PropEntry
{
    public GameObject prefab; 
    [Range(0f, 10)]
    public int probability = 5;
}

public class PropGenerator : MonoBehaviour
{
    [SerializeField] private string generatorName = "";
    [Header("Props To Spawn")]
    [SerializeField] private List<PropEntry> props;

    [Header("Cluster Settings")]
    [SerializeField] private float frequency = 0.05f;
    [SerializeField] private int minClusterSize = 2;
    [SerializeField] private int maxClusterSize = 5;
    [SerializeField] private int clusterRadius = 2;

    [Header("Misc")]
    [SerializeField] private bool avoidOverlap = true;

    private PropManager propMan;

    private void Awake()
    {
        propMan = transform.parent.GetComponent<PropManager>();
        if (propMan != null)
            propMan.GenerateProps.AddListener(GenerateProps);
    }

    public void GenerateProps(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
    {
        if (props == null || props.Count == 0)
        {
            Debug.LogWarning("No props assigned for " + generatorName);
            return;
        }

        GameObject propParent = new GameObject("props_" + generatorName);
        propParent.transform.parent = transform;

        Dictionary<Vector2Int, GameObject> propList = new Dictionary<Vector2Int, GameObject>();

        foreach (Vector2Int tile in floorTiles)
        {
            if (Random.value > frequency)
                continue;

            int clusterSize = Random.Range(minClusterSize, maxClusterSize + 1);

            for (int i = 0; i < clusterSize; i++)
            {
                Vector2Int offset = new Vector2Int(
                    Random.Range(-clusterRadius, clusterRadius + 1),
                    Random.Range(-clusterRadius, clusterRadius + 1)
                );

                Vector2Int spawnTile = tile + offset;

                if (!floorTiles.Contains(spawnTile)) continue;
                if (wallTiles.Contains(spawnTile)) continue;
                if (avoidOverlap && propList.ContainsKey(spawnTile)) continue;

                GameObject prefab = ChoosePropByProbability(props.ToList());

                if (prefab == null) continue;

                Vector3 spawnPos = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);
                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, propParent.transform);

                propList[spawnTile] = obj;
            }
        }

        if (propMan != null)
            propMan.props.Add(propList);
    }
    private GameObject ChoosePropByProbability(List<PropEntry> props)
    {
        float total = 0f;
        foreach (var entry in props)
        {
            total += entry.probability;
        }

        float r = Random.value * total;
        float cumulative = 0f;

        foreach (var entry in props)
        {
            cumulative += entry.probability;
            if (r <= cumulative)
                return entry.prefab;
        }

        return props[props.Count - 1].prefab; 
    }

}
