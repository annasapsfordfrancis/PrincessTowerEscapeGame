using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { START, PLAYERTURN, ENEMYTURN, WIN, LOSE, PAUSE }
public class TurnSystem : MonoBehaviour
{
    public GameObject UI;
    public GameObject playerPrefab;
    public GameObject Player;
    public GameObject PlayerSpawnPoint;
    public GameObject CurrentEnemy;
    public List<GameObject> EnemiesInPlay;
    public static TurnState state;
    public static TurnState previousState;
    
    private HUD HUD;
    private PlayerStats playerStats;
    private PlayerManager playerManager;
    public int currentEnemyIndex;

    public delegate void OnWin();
    public OnWin onWinCallback;
    public delegate void OnLose();
    public OnLose onLoseCallback;
    public delegate void OnPause();
    public OnPause onPauseCallback;

    void Start()
    {
        HUD = UI.GetComponent<HUD>();
        state = TurnState.START;
        HUD.ShowMessage();
        GenerateLevel();
    }

    void GenerateLevel()
    {
        Player = Instantiate(playerPrefab, PlayerSpawnPoint.transform.position, Quaternion.identity);
        playerStats = Player.GetComponent<PlayerStats>();
        playerManager = Player.GetComponent<PlayerManager>();
    }

    public void RegisterEnemy(GameObject enemy)
    {
        EnemiesInPlay.Add(enemy);
    }

    public void StartPlayerTurn()
    {
        playerManager.StartTurn();
    }

    public void EndPlayerTurn()
    {
        StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        if (EnemiesInPlay.Count > 0)
        {
            // Set current enemy to 1st item in enemy list
            currentEnemyIndex = 0;
            CurrentEnemy = EnemiesInPlay[currentEnemyIndex];
            state = TurnState.ENEMYTURN;
            
        }
        else
        {
            StartPlayerTurn();
        }
    }

    public void EndEnemyTurn()
    {
        currentEnemyIndex++;

        // If index is out of bounds
        if (currentEnemyIndex == EnemiesInPlay.Count)
        {
            currentEnemyIndex = 0;
            CurrentEnemy = null;
            StartPlayerTurn();
        }
        else if (currentEnemyIndex < EnemiesInPlay.Count)
        {
            DoNextEnemyTurn();
        }
    }

    public void DoNextEnemyTurn()
    {
        CurrentEnemy = EnemiesInPlay[currentEnemyIndex];
    }

    public void PlayerDefeated()
    {
        state = TurnState.LOSE;
        if (onLoseCallback != null)
        {
            onLoseCallback.Invoke();
        }
    }

    public void PlayerWin()
    {
        state = TurnState.WIN;
        if (onWinCallback != null)
        {
            onWinCallback.Invoke();
        }
    }

    public void PauseGame()
    {
        previousState = state;
        state = TurnState.PAUSE;
        if (onPauseCallback != null)
        {
            onPauseCallback.Invoke();
        } 
    }

    public void ResumeGame()
    {
        state = previousState;
    }

}
