using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesSpawner : MonoBehaviour
{
    public Enemy EnemyPrefab = null;
    public Vector2 SpawnIntervalMinMax = new Vector2(1f,5f);
    public int MaxSpawned = 3;

    float toSpawnTimer = 0.0f;
    int spawnedCounter = 0;

    List<Enemy> spawnedEnemies;// = new ArrayList();

    // Use this for initialization
    void Start()
    {
        spawnedEnemies = new List<Enemy>(MaxSpawned);
        
        ResetSpawTimer();
        spawnedCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        toSpawnTimer -= Time.deltaTime;
        if( toSpawnTimer <= 0f )
        {
            if( spawnedEnemies.Count < MaxSpawned)
            {
                Spawn();
            }
        }
    }

    Enemy Spawn()
    {
        Enemy newEnemy = Instantiate<Enemy>(EnemyPrefab);
        newEnemy.Spawner = this;
        newEnemy.transform.position = transform.position;
        spawnedEnemies.Add(newEnemy);
        ResetSpawTimer();
        return newEnemy;
    }

    void ResetSpawTimer()
    {
        toSpawnTimer = Random.Range(SpawnIntervalMinMax.x, SpawnIntervalMinMax.y);
    }

    public void NurslingFall(Enemy zombie)
    {
        spawnedEnemies.Remove(zombie);
    }

    public void Reset()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            Destroy(enemy.gameObject);
        }
        spawnedEnemies.Clear();
        ResetSpawTimer();
    }
}
