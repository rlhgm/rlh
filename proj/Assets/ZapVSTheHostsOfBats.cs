using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZapVSTheHostsOfBats : MonoBehaviour {

    public Transform[] batsStartPos;
    public BatActivator batActivator = null;
    //List<Bat> spawnedBats;
    public Bat batPrefab = null;
    int level = 1;

    // Use this for initialization
    void Start () {
        level = 1;
        createBatWave();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ZapDead()
    {
        
    }
    public void ZapReborn()
    {
        for (int i = 0; i < batActivator.bats.Count; ++i)
        {
            Destroy(batActivator.bats[i]);
        }
        batActivator.bats.Clear();

        //createBatWave();
    }
    public void BatDead(Bat deadBat)
    {
        batActivator.bats.Remove(deadBat);
        if(batActivator.bats.Count == 0 )
        {
            level++;
            createBatWave();
        }
    }

    void createBatWave()
    {
        //spawnedBats = new List<Bat>(batsStartPos.Length * level);

        for (int i = 0; i < batsStartPos.Length; ++i)
        {
            for (int l = 0; l < level; ++l)
            {
                Bat newBat = Instantiate<Bat>(batPrefab);
                newBat.Activator = batActivator;
                batActivator.bats.Add(newBat);
                newBat.transform.position = batsStartPos[i].transform.position;
                //spawnedBats.Add(newBat);
            }
        }
    }
}
