using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActionManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject rangeButton;

    [SerializeField] private Transform border;
    [SerializeField] private Transform enemyBorder;

    [SerializeField] private float meleeDistance = 1f;
    [SerializeField] private float enemyTimeoutDuration = 3f;

    private List<Player> allPlayers = new List<Player>();
    private Player currentPlayer;
    private Player target;

    private Coroutine currentActionCoroutine;

    private IconManager iconManager;

    private int round = 0;

    public bool HasChosenAction { private set; get; }

    public void Initialize(List<Player> players)
    {
        allPlayers.Clear();
        allPlayers.AddRange(players);

        iconManager = new IconManager(allPlayers, meleeDistance);
    }

    public void SetCurrentPlayer(Player currentPlayer)
    {
        this.currentPlayer = currentPlayer;
        HasChosenAction = false;

        iconManager.ResetIcons();

        border.gameObject.SetActive(!currentPlayer.Enemy);
        enemyBorder.gameObject.SetActive(currentPlayer.Enemy);

        if (currentPlayer.Enemy)
        {
            enemyBorder.position = currentPlayer.GetStatBoxPosition();
            StartCoroutine(EnemyAction());
            return;
        }

        border.position = currentPlayer.GetStatBoxPosition();
        rangeButton.SetActive(currentPlayer.GetCanRangeAttack());
    }


    public void OnRangeAttack()
    {
        iconManager.SetIconsInRange(currentPlayer);

        if (currentActionCoroutine != null)
            StopCoroutine(currentActionCoroutine);

        currentActionCoroutine = StartCoroutine(PlayerAction(currentPlayer.RangeAttack));
    }

    public void OnMeleeAttack()
    {
        iconManager.SetIconsInMeleeRange(currentPlayer);

        if (currentActionCoroutine != null)
            StopCoroutine(currentActionCoroutine);

        currentActionCoroutine = StartCoroutine(PlayerAction(currentPlayer.MeleeAttack));
    }

    public void OnHeal()
    {
        iconManager.SetIconsInHealRange(currentPlayer);

        if (currentActionCoroutine != null)
            StopCoroutine(currentActionCoroutine);

        currentActionCoroutine = StartCoroutine(PlayerAction(currentPlayer.Heal));
    }

    public void OnChooseTarget(Player target)
    {
        this.target = target;
    }

    private IEnumerator PlayerAction(Action<Player> action)
    {
        target = null;
        gameManager.IsWaitingForMovement = false;

        while (target == null)
        {
            yield return new WaitForEndOfFrame();
        }

        action(target);
        HasChosenAction = true;
        currentActionCoroutine = null;
    }

    private IEnumerator EnemyAction()
    {
        float startTime = Time.time;

        yield return new WaitForSeconds(1);

        gameManager.IsWaitingForMovement = false;

        yield return new WaitForSeconds(1);

        OnMeleeAttack();

        yield return WaitUntilTargetSelectedOrTimeout(enemyTimeoutDuration, startTime);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator WaitUntilTargetSelectedOrTimeout(float duration, float startTime)
    {
        while (target == null && Time.time - startTime < duration)
        {
            ChooseRandomTarget();
            yield return new WaitForEndOfFrame();
        }

        if (target == null)
        {
          

            HasChosenAction = true;
            currentActionCoroutine = null;
        }
    }

    private void ChooseRandomTarget()
    {
        Player closestTarget = FindClosestTarget(meleeDistance);

        if (closestTarget == null)
        {
            OnRangeAttack();
            closestTarget = FindClosestTarget(currentPlayer.GetMaxAttackRange());
        }

        if (closestTarget != null)
        {
            round++;

          
        }

        target = closestTarget;
    }

    private Player FindClosestTarget(float maxRange)
    {
        Player closestTarget = null;
        float minDistance = float.MaxValue;
        List<Player> closestTargets = new List<Player>();

        foreach (Player targetPlayer in allPlayers)
        {
            if (!IsValidTarget(targetPlayer))
                continue;

            float distance = Vector2.Distance(currentPlayer.transform.position, targetPlayer.transform.position);

            if (distance <= maxRange)
            {
                UpdateClosestTargets(distance, targetPlayer, ref minDistance, ref closestTargets);
            }
        }

        if (closestTargets.Count > 0)
        {
            closestTarget = closestTargets[Random.Range(0, closestTargets.Count)];
        }

        return closestTarget;
    }

    private bool IsValidTarget(Player player)
    {
        return !player.Enemy;
    }

    private void UpdateClosestTargets(float distance, Player targetPlayer, ref float minDistance, ref List<Player> closestTargets)
    {
        if (distance < minDistance)
        {
            minDistance = distance;
            closestTargets.Clear();
            closestTargets.Add(targetPlayer);
        }

        else if (distance == minDistance)
        {
            closestTargets.Add(targetPlayer);
        }
    }
}
