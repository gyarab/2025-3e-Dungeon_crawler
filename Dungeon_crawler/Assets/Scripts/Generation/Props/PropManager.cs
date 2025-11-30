using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropManager : MonoBehaviour
{
    public List<Dictionary<Vector2Int, GameObject>> props = new List<Dictionary<Vector2Int, GameObject>>();
    [HideInInspector] public UnityEvent<HashSet<Vector2Int>, HashSet<Vector2Int>> GenerateProps;
    public void GenerateAllProps(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
    {
        Debug.Log("Generating props...");
        GenerateProps?.Invoke(floorTiles, wallTiles);
        StartCoroutine(WaitAndCheckOverlaps());
    }

    private IEnumerator WaitAndCheckOverlaps()
    {
        yield return new WaitForSeconds(0.5f);
        CheckOverlaps();
    }

    public void CheckOverlaps()
    {
        HashSet<Vector2Int> seenTiles = new HashSet<Vector2Int>();

        foreach (var dict in props)
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();

            foreach (var kvp in dict)
            {
                Vector2Int tile = kvp.Key;
                GameObject obj = kvp.Value;

                if (seenTiles.Contains(tile))
                {
                    if (obj != null)
                        Destroy(obj);

                    toRemove.Add(tile);
                }
                else
                {
                    seenTiles.Add(tile);
                }
            }

            foreach (Vector2Int t in toRemove)
            {
                dict.Remove(t);
            }
        }
    }

}
