using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private List<Vector2Int> assignedPositions = new List<Vector2Int>();
    public virtual Vector2Int GetStartPosition()
    {
        Vector2Int position;

        do
        {
            position = GetRandomPosition();

        } while (assignedPositions.Contains(position));

        assignedPositions.Add(position);
        return position;
    }

    private Vector2Int GetRandomPosition()
    {
        int randomRow = Random.Range(0, Configuration.GridHeight);
        int randomColumn = Random.Range(0, Configuration.GridWidth);

        return new Vector2Int(randomRow, randomColumn);
    }

    public virtual List<List<TerrainType>> GenerateMap()
    {
        var map = new List<List<TerrainType>>();

        for (var width = 0; width < Configuration.GridWidth; width++)
        {
            var row = new List<TerrainType>();
            for (var height = 0; height < Configuration.GridHeight; height++)
            {
                row.Add(TerrainType.GRASS);
            }

            map.Add(row);
        }

        return map;
    }
}
