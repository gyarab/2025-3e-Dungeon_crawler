using UnityEngine;
using System.Collections.Generic;

public class TorchGenerator : MonoBehaviour
{
    public GameObject torchPrefab;
    public float torchChance = 0.05f;

    private PropManager propMan;

    public void Awake()
    {
        propMan = transform.parent.GetComponent<PropManager>();
        propMan.GenerateProps.AddListener(GenerateProps);
    }

    public void GenerateProps(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> wallTiles)
    {
        GameObject torchParent = Instantiate(new GameObject("Torches"), transform);
        //Debug.Log("props:");
        //Debug.Log(" - torch");
        foreach (var tile in floorTiles)
        {
            if (Random.value > torchChance)
                continue;

            Vector2Int[] dirs =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            bool nearWall = false;

            foreach (var d in dirs)
            {
                if (wallTiles.Contains(tile + d))
                {
                    nearWall = true;
                    break;
                }
            }

            if (!nearWall) continue;

            GameObject torch = Instantiate(torchPrefab, new Vector3(tile.x, tile.y, 0), Quaternion.identity);
            torch.transform.parent = torchParent.transform;
        }
    }
}
