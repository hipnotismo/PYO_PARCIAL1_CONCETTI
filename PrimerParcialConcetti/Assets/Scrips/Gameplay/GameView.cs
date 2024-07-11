using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    private List<List<GameObject>> grid;

    [SerializeField] private float gridCellSize = 1f;
    [SerializeField] private float spaceBetweenCells = 0.1f;
    [SerializeField] private GameObject grassRef;

    private GameObject player;
    private List<GameObject> playerObjects = new List<GameObject>();

    public void SetPlayers(List<GameObject> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerObjects.Clear();
            playerObjects.AddRange(players);

            playerObjects[i].SetActive(true);
        }
    }

    public void SetCurrentPlayer(GameObject currentPlayer)
    {
        player = currentPlayer;
    }
    public void MovePlayerToCell(int row, int column)
    {
        MovePlayerToCell(grid[column][row]);
    }
    private void MovePlayerToCell(GameObject gridCell)
    {
        player.transform.SetParent(gridCell.transform);
        player.transform.localPosition = Vector3.zero;
    }

    public void InitializeCharacterPositions(List<Vector2Int> playerPositions)
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            player = playerObjects[i];
            MovePlayerToCell(playerPositions[i].x, playerPositions[i].y);
        }
        player = playerObjects[0];
    }

    public void InitializeMap(List<List<TerrainType>> map)
    {
        grid = new List<List<GameObject>>();

        for (var row = 0; row < map.Count; row++)
        {
            var gridRow = new List<GameObject>();
            for (var column = 0; column < map[row].Count; column++)
            {
                var xPos = column * (gridCellSize + spaceBetweenCells);
                var yPos = row * (gridCellSize + spaceBetweenCells);
               
                var reference = Instantiate(grassRef, transform);

              

                reference.transform.localPosition = new Vector3(xPos, yPos, 1);
                gridRow.Add(reference);
            }
            grid.Add(gridRow);
        }
    }
}
