using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    private List<Player> playerTurn;
    private Player currentPlayer;

    private Mover mover;
    [SerializeField] private GameView gameView;

    void Start()
    {
        currentPlayer = players[0];

        gameView.SetCurrentPlayer(currentPlayer.gameObject);
        gameView.SetPlayers(players.ConvertAll(p => p.gameObject));

        mover = new Mover(gameView, new MapBuilder(),players.Count);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mover.MoveCharacterRight();
            Debug.Log("here");
        }
    }
}
