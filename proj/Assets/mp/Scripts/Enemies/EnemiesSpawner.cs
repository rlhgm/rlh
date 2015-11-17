using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesSpawner : MonoBehaviour
{
    public Enemy EnemyPrefab = null;
    public Vector2 SpawnIntervalMinMax = new Vector2(1f,5f);
    public int MaxSpawned = 3;

    float toSpawnTimer = 0.0f;
    List<Enemy> spawnedEnemies;

    // Use this for initialization
    void Start()
    {
        spawnedEnemies = new List<Enemy>(MaxSpawned);
        ResetSpawTimer();
    }

    // Update is called once per frame
    void Update()
    {
        toSpawnTimer -= Time.deltaTime;
        if (CanSpawnNext()) Spawn();
    }

    Enemy Spawn()
    {
        Enemy newEnemy = Instantiate<Enemy>(EnemyPrefab);
        newEnemy.Spawner = this;
        newEnemy.transform.position = transform.position;
        spawnedEnemies.Add(newEnemy);
        ResetSpawTimer();
        RLHScene.Instance.RatBorn(newEnemy.GetComponent<Rat>());
        return newEnemy;
    }

    bool CanSpawnNext()
    {
        if (toSpawnTimer > 0f) return false;
        if (spawnedEnemies.Count >= MaxSpawned) return false;
        if (!CheckExitIsOpen()) return false;
        return true;
    }

    bool CheckExitIsOpen()
    {
        return true;
    } 

    void ResetSpawTimer()
    {
        toSpawnTimer = Random.Range(SpawnIntervalMinMax.x, SpawnIntervalMinMax.y);
    }

    public void NurslingFall(Enemy zombie)
    {
        Destroy(zombie.gameObject,2f);
        spawnedEnemies.RemoveAt(spawnedEnemies.IndexOf(zombie));
    }

    public void Reset()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy)
            {
                RLHScene.Instance.RatDie(enemy.GetComponent<Rat>());
                Destroy(enemy.gameObject);
            }
        }
        // to w sumie wolane jest juz z zapa...
        //RLHScene.Instance.ResetRats();
        spawnedEnemies.Clear();
        ResetSpawTimer();
    }
}
