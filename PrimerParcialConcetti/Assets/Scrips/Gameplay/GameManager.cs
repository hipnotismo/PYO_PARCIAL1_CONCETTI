using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;
    [SerializeField] private ActionManager actionManager;

    private Player currentPlayer;

    private bool[] playerDeadFlags;


    private int playersTotal = 0;
    private int playerCounter = 0;
    private int enemiesCounter = 0;

    private int turn = 1;
    private int maxTurns = 0;

    private Mover mover;

    private bool gameOver = false;

    [SerializeField] private GameView gameView;

    public bool IsWaitingForMovement { set; get; }

    private void OnEnable()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].onDead += KillCounter;
        }
    }

    void Start()
    {
        currentPlayer = players[0];

        gameView.SetCurrentPlayer(currentPlayer.gameObject);
        gameView.SetPlayers(players.ConvertAll(p => p.gameObject));

        actionManager.Initialize(players);
        actionManager.SetCurrentPlayer(currentPlayer);

        maxTurns = players.Count;

        playerDeadFlags = new bool[players.Count];


        for (int i = 0; i < playerDeadFlags.Length; i++)
        {
            playerDeadFlags[i] = false;

            if (players[i].Enemy) enemiesCounter++;
            else
            {
                playerCounter++;
                playersTotal++;
            }
        }

        mover = new Mover(gameView, new MapBuilder(),players.Count);
        StartCoroutine(PlayerTurn());

    }

    void Update()
    {
        if (!IsWaitingForMovement || gameOver) return;

        if (currentPlayer.Enemy)
        {
            mover.MoveEnemyRandomly();
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                mover.MoveCharacterLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow) )
                mover.MoveCharacterRight();
            else if (Input.GetKeyDown(KeyCode.UpArrow) )
                mover.MoveCharacterUp();
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                mover.MoveCharacterDown();
        }
       
    }

    private IEnumerator PlayerTurn()
    {
        if (gameOver) yield break;

        bool currentPlayerIsDead = IsPlayerDead(turn);

        if (!currentPlayerIsDead)
        {
            UpdateCharacter();

            mover.UpdateCharacterPosition(turn);

            yield return WaitForMovement();
            yield return WaitForAction();

            mover.StoreCharacterPosition(turn);
        }

        CheckIfGameOver();
        turn = (turn % maxTurns) + 1;

        StartCoroutine(PlayerTurn());
    }

    private bool IsPlayerDead(int playerTurn)
    {
        return playerDeadFlags[playerTurn - 1];
    }

    private void UpdateCharacter()
    {
        currentPlayer = players[turn - 1];

        gameView.SetCurrentPlayer(currentPlayer.gameObject);
        actionManager.SetCurrentPlayer(currentPlayer);
    }

    private IEnumerator WaitForMovement()
    {
        IsWaitingForMovement = true;

        while (mover.Speed < players[turn - 1].GetMaxSpeed() && IsWaitingForMovement == true)
        {
            yield return new WaitForEndOfFrame();
        }

        IsWaitingForMovement = false;
    }

    private IEnumerator WaitForAction()
    {
        while (!actionManager.HasChosenAction)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckIfGameOver()
    {
        if (playerCounter == 1)
        {
            Debug.Log("PLAYER " + turn + " WINS!!!");

            gameOver = true;

            actionManager.gameObject.SetActive(false);
        }

        else if (playerCounter < playersTotal && enemiesCounter > 0)
        {
            
            Debug.Log("EVERYBODY LOSES!!!");

            gameOver = true;

            actionManager.gameObject.SetActive(false);
        }
    }

    private void KillCounter()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].health == 0)
            {               
                Debug.Log("Player " + (i + 1) + " has died.");

                playerDeadFlags[i] = true;

                if (players[i].Enemy)
                {
                    enemiesCounter--;
                }

                else
                {
                    playerCounter--;
                }

                mover.RemovePositionAfterDeath(i, turn);
            }
        }
    }
}
