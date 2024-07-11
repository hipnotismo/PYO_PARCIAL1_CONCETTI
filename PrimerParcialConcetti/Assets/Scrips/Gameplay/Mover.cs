using System.Collections.Generic;
using UnityEngine;

public class Mover 
{

    private List<List<TerrainType>> map;

    private GameView gameView;

    private Vector2Int currentPlayerPosition;
    private List<Vector2Int> positions = new List<Vector2Int>();

    public int Speed { private set; get; }

    public Mover(GameView view, MapBuilder mapBuilder, int playerAmount)
    {
        gameView = view;

        map = mapBuilder.GenerateMap();
        gameView.InitializeMap(map);

        for (int i = 0; i < playerAmount; i++)
        {
            Vector2Int newPosition = mapBuilder.GetStartPosition();
            positions.Add(newPosition);
            map[newPosition.y][newPosition.x] = TerrainType.CHARACTER;
        }

        currentPlayerPosition = positions[0];
        gameView.InitializeCharacterPositions(positions);
    }

    public void MoveCharacterRight()
    {
        if (IsValidPosition(currentPlayerPosition.x + 1, currentPlayerPosition.y))
        {
            MoveCharacterToPosition(currentPlayerPosition.x + 1, currentPlayerPosition.y);
        }
    }

    public void MoveCharacterLeft()
    {
        if (IsValidPosition(currentPlayerPosition.x - 1, currentPlayerPosition.y))
        {
            MoveCharacterToPosition(currentPlayerPosition.x - 1, currentPlayerPosition.y);
        }
    }

    public void MoveCharacterUp()
    {
        if (IsValidPosition(currentPlayerPosition.x, currentPlayerPosition.y + 1))
        {
            MoveCharacterToPosition(currentPlayerPosition.x, currentPlayerPosition.y + 1);
        }
    }

    public void MoveCharacterDown()
    {
        if (IsValidPosition(currentPlayerPosition.x, currentPlayerPosition.y - 1))
        {
            MoveCharacterToPosition(currentPlayerPosition.x, currentPlayerPosition.y - 1);
        }
    }

    private void MoveCharacterToPosition(int newX, int newY)
    {
        if (newX == currentPlayerPosition.x && newY == currentPlayerPosition.y)
        {
            return;
        }

        map[currentPlayerPosition.y][currentPlayerPosition.x] = TerrainType.GRASS;
        map[newY][newX] = TerrainType.CHARACTER;

        currentPlayerPosition = new Vector2Int(newX, newY);

        gameView.MovePlayerToCell(currentPlayerPosition.x, currentPlayerPosition.y);
        Speed++;
    }

    private bool IsValidPosition(int x, int y)
    {
        return PositionExistsInMap(x, y) && ThereIsNoCharacterInPosition(x, y);
    }

    private bool PositionExistsInMap(int x, int y)
    {
        return y >= 0 &&
               y < map.Count &&
               x >= 0 &&
               x < map[y].Count;
    }

    private bool ThereIsNoCharacterInPosition(int x, int y)
    {
        return map[y][x] == TerrainType.GRASS;
    }
}
