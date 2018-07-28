/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Entity
{
    [Serializable]
    public class Wave
    {
        public EntityPrefab[] entities;
    }

    [Serializable]
    public class EntityPrefab
    {
        public Enemy Prefab;
        public int NumberToSpawn = 10;
    }

    [Header("Spawner")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private int waveToRepeat = -1;

    /// <summary>
    /// Starts the spawn resolver
    /// </summary>
    protected override void Start()
    {
        base.Start();
        EnemyManager.OnWaveStart += EnemyManager_OnWaveStart;
	}

    /// <summary>
    /// Starts the spawning of a new wave
    /// </summary>
    private void EnemyManager_OnWaveStart(int waveNumber)
    {
        // If we're forcibly setting the wave number, use that instead of the one passed in
        if (waveToRepeat != -1)
        {
            waveNumber = waveToRepeat;
        }

        // Start spawning the wave
        StartCoroutine(StartWave(waveNumber));
    }

    /// <summary>
    /// Spawns enemies after the specified delay
    /// </summary>
    private IEnumerator StartWave(int waveNumber)
    {
        // TODO: Modify this to spawn enemies randomly

        for (int i = 0; i < waves[waveNumber].entities.Length; i++)
        {
            for (int j = 0; j < waves[waveNumber].entities[i].NumberToSpawn; j++)
            {
                SpawnEntity(waves[waveNumber].entities[i]);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    /// <summary>
    /// Spawns the given prefab at this objects location
    /// </summary>
    /// <param name="enemy">The enemy to spawn</param>
    private void SpawnEntity(EntityPrefab entity)
    {
        Enemy enemy = GameObject.Instantiate(entity.Prefab, transform.position, Quaternion.identity, transform);
        EnemyManager.Instance.ActiveEnemies.Add(enemy);
    }

    /// <summary>
    /// A simple function that returns the total number of enemies this spawner will spawn per wave
    /// </summary>
    /// <returns>The total number of enemies spawned per wave by this spawner</returns>
    public int TotalEnemiesToSpawn()
    {
        int totalEnemies = 0;

        // Determine the wave number to use
        int currentWave = EnemyManager.Instance.WaveNumber;
        if (waveToRepeat != -1)
        {
            currentWave = waveToRepeat;
        }

        // Tally up the number of enemies per entity prefab
        for (int i = 0; i < waves[currentWave].entities.Length; i++)
        {
            totalEnemies += waves[currentWave].entities[i].NumberToSpawn;
        }

        return totalEnemies;
    }
}
