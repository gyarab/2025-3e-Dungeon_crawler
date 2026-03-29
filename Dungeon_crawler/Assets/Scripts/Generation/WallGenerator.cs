using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WallGenerator : MonoBehaviour
{
    public List<HashSet<Vector2Int>> GenerateWalls(Dictionary<Vector2Int,bool> floorTiles, int width)
    {
        Dictionary<Vector2Int, bool> wallTiles = new Dictionary<Vector2Int, bool>();

        List<Vector2Int> dir = new List<Vector2Int>();
        //list of directions
        dir.Add(new Vector2Int(1, 0));
        dir.Add(new Vector2Int(-1, 0));
        dir.Add(new Vector2Int(0, 1));
        dir.Add(new Vector2Int(0, -1));


        //if neighbor tile isnt in hashset of tiles, its a wall
        foreach (Vector2Int tile in floorTiles.Keys)
        {
            foreach (Vector2Int d in dir)
            {
                Vector2Int neighbour = tile + d;
                if (!floorTiles.ContainsKey(neighbour)&&!wallTiles.ContainsKey(neighbour))
                {
                    if (floorTiles[tile])
                    {
                        wallTiles.Add(neighbour, true);
                    }
                    else
                    {
                        wallTiles.Add(neighbour, false);
                    }
                }
            }
        }
        Dictionary<Vector2Int, bool> thickWalls = new Dictionary<Vector2Int, bool>(wallTiles);

        for (int i = 0; i < width; i++)
        {
            Dictionary<Vector2Int, bool> tempThickWalls = new Dictionary<Vector2Int, bool>(thickWalls);
            foreach (Vector2Int wall in tempThickWalls.Keys)
            {
                foreach (Vector2Int d in dir)
                {
                    Vector2Int neighbour = wall + d;
                    if (!thickWalls.ContainsKey(neighbour) && !wallTiles.ContainsKey(neighbour)&& !floorTiles.ContainsKey(neighbour))
                    {
                        if (thickWalls[wall])
                        {
                            thickWalls.Add(neighbour, true);
                        }
                        else
                        {
                            thickWalls.Add(neighbour, false);
                        }
                    }
                }
            }
        }
        foreach (Vector2Int wall in thickWalls.Keys)
        {
            if (!wallTiles.ContainsKey(wall))
            {
                wallTiles.Add(wall, thickWalls[wall]);
            }
        }

        List <HashSet<Vector2Int>> result = new List<HashSet<Vector2Int>>();
        result.Add(new HashSet<Vector2Int>(wallTiles.Where(r => r.Value == false).Select(r => r.Key)));
        result.Add(new HashSet<Vector2Int>(wallTiles.Where(r => r.Value == true).Select(r => r.Key)));

        return result;
    }
}
