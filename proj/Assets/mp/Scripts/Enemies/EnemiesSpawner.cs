using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesSpawner : MonoBehaviour
{
    public Enemy EnemyPrefab = null;
    public Vector2 SpawnIntervalMinMax = new Vector2(1f, 5f);
    public int MaxSpawned = 3;
    public bool On = true;

    float toSpawnTimer = 0.0f;
    List<Enemy> spawnedEnemies;
    BoxCollider2D boxCollider;

    Vector2 topLeft;
    Vector2 topRight;
    Vector2 btmLeft;
    Vector2 btmRight;

    // Use this for initialization
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        findWorldVertices();
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
        if (!On) return false;
        if (toSpawnTimer > 0f) return false;
        if (spawnedEnemies.Count >= MaxSpawned) return false;
        if (!CheckExitIsOpen()) return false;
        return true;
    }

    RaycastHit2D hit;
    bool CheckExitIsOpen()
    {
        hit = Physics2D.Linecast(btmLeft, topRight, RLHScene.Instance.layerIdGroundAllMask);
        if (hit.collider) return false;
        hit = Physics2D.Linecast(topLeft, btmRight, RLHScene.Instance.layerIdGroundAllMask);
        if (hit.collider) return false;
        return true;
    }

    public void findWorldVertices()
    {
        float top = boxCollider.offset.y + (boxCollider.size.y * 0.5f);
        float btm = boxCollider.offset.y - (boxCollider.size.y * 0.5f);
        float left = boxCollider.offset.x - (boxCollider.size.x * 0.5f);
        float right = boxCollider.offset.x + (boxCollider.size.x * 0.5f);

        topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
        topRight = transform.TransformPoint(new Vector3(right, top, 0f));
        btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
        btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));

        //Debug.Log(topLeft + " " + topRight + " " + btmLeft + " " + btmRight);

        //float rot = transform.rotation.eulerAngles.z;
    }

    void ResetSpawTimer()
    {
        toSpawnTimer = Random.Range(SpawnIntervalMinMax.x, SpawnIntervalMinMax.y);
    }

    public void NurslingFall(Enemy zombie)
    {
        Destroy(zombie.gameObject, 2f);
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

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    print(other.name);
    //}
}
