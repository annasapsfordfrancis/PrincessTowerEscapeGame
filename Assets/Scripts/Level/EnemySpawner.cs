using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int EnemiesSpawnedMinimum = 1;

    // Maximum 9
    public int EnemiesSpawnedMaximum = 3;
    public List<GameObject> EnemyList;
    public List<GameObject> SquadList;
    private List<Vector2> locationList;
    private TurnSystem turnSystem;
    void Start()
    {
        locationList = new List<Vector2>{Vector2.up, Vector2.left, Vector2.right, Vector2.down, Vector2.zero, new Vector2(-1, 1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, -1)};
        turnSystem = GameObject.Find("TurnSystem").GetComponent<TurnSystem>();
        SquadList = new List<GameObject>();
        // Clamp EnemiesSpawnedMaximum to 9
        if (EnemiesSpawnedMaximum > 9) EnemiesSpawnedMaximum = 9;
        SpawnEnemies();
    }

    // Draws a square in inspector where enemies will be spawned
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(3f,3f,0f));
    }

    void SpawnEnemies()
    {
        int enemiesSpawned = Random.Range(EnemiesSpawnedMinimum, EnemiesSpawnedMaximum + 1);

        int i = 0;
        while (i < enemiesSpawned)
        {
            // Instantiate enemies at relative locations in locationList
            GameObject enemy = GameObject.Instantiate(EnemyList[0], locationList[i] += (Vector2)transform.position, Quaternion.identity);
            turnSystem.RegisterEnemy(enemy);
            SquadList.Add(enemy);
            enemy.GetComponent<EnemyAI>().squadList = SquadList;
            i++;
        }
    }
}
