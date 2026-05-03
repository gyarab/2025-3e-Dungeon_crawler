using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropManager : MonoBehaviour
{
    public Dictionary<Vector2Int, GameObject> props = new Dictionary<Vector2Int, GameObject>();
    [HideInInspector] public UnityEvent<HashSet<Vector2Int>, HashSet<Vector2Int>> GenerateProps;
    [HideInInspector] public UnityEvent ClearProps;

    public IEnumerator GenerateAllProps(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
    {
        Debug.Log("Generating props...");
        yield return new WaitForSeconds(0.1f);
        GenerateProps?.Invoke(floorTiles, wallTiles);
    }

    public void GenerateSetProps(Dictionary<Vector2Int, GameObject> propsToGenerate, Room room)
    {
        Vector2Int bspCenter = room.center;
        Vector2Int prefabCenter = room.prefabRoomSO.center;
        Vector2Int centerOffset = bspCenter - prefabCenter;

        foreach (var kvp in propsToGenerate)
        {
            Vector2Int world = kvp.Key + centerOffset;
            GameObject obj = kvp.Value;

            if (obj != null)
            {
                GameObject newObj = Instantiate(
                    obj,
                    new Vector3(world.x, world.y, 0),
                    Quaternion.identity
                );

                newObj.transform.parent = transform;
                props.Add(world, newObj);
            }
        }
    }

    public void MergePropDict(Dictionary<Vector2Int, GameObject> propsToAdd)
    {
        foreach (var kvp in propsToAdd)
        {
            if (!props.ContainsKey(kvp.Key))
                props.Add(kvp.Key, kvp.Value);
        }
    }

    public void ClearAllProps()
    {
        foreach (GameObject prop in props.Values)
        {
            if (prop != null)
                Destroy(prop);
        }
        props.Clear();
        ClearProps?.Invoke();
    }
}
