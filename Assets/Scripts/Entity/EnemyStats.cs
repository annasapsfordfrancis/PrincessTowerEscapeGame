using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public string EntityName;
    public int AttackDamage;
    public int MaxHP;
    public int CurrentHP;
    public int AggroDistance;
    public int DisengageDistance;
    public int MeleeDistance = 1;
    private LootTable lootTable;
    private TurnSystem turnSystem;
    private EnemyAI enemyAI;

    void Start()
    {
        turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        lootTable = GetComponent<LootTable>();
        enemyAI = GetComponent<EnemyAI>();
        
        // Make sure DisengageDistance is at least equal to AggroDistance to avoid buggy behaviour
        if (DisengageDistance < AggroDistance)
        {
            DisengageDistance = AggroDistance;
        }
    }

    public void UpdateHP(int amount)
    {
        int oldHP = CurrentHP;
        CurrentHP += amount;
        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }
        else if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            enemyAI.AlertSquad();
            turnSystem.EnemiesInPlay.Remove(this.gameObject);
            enemyAI.squadList.Remove(this.gameObject);
            lootTable.DropItem();
            transform.gameObject.SetActive(false);
        }
        // Aggro enemy if it was damaged and it didn't kill them
        else
        {
            enemyAI.AggroSelf();
            enemyAI.AlertSquad();
        }
    }

    public void DealDamage(int damage, Transform target)
    {
        PlayerStats targetStats = target.GetComponent<PlayerStats>();
        targetStats.TakeDamage(-damage);
    }
}
