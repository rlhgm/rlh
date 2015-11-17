using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    public abstract void Reset();

    protected EnemiesSpawner spawner;
    public EnemiesSpawner Spawner
    {
        get
        {
            return spawner;
        }

        set
        {
            spawner = value;
        }
    }
}
