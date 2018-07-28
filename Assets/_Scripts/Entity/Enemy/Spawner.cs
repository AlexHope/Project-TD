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
    public class EntityPrefab
    {
        public Enemy Prefab;
        public int NumberToSpawn = 10;
    }

    [Header("Spawner")]
    [SerializeField] private EntityPrefab[] entityPrefabs;
    [SerializeField] private float spawnDelay = 0.5f;

	/// <summary>
    /// Starts the spawn resolver
    /// </summary>
	protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(SpawnResolver());
	}

    /// <summary>
    /// Spawns enemies after the specified delay
    /// </summary>
    private IEnumerator SpawnResolver()
    {
        yield return new WaitForSeconds(1.0f);
        while (Application.isPlaying)
        {
            for (int i = 0; i < entityPrefabs.Length; i++)
            {
                for (int j = 0; j < entityPrefabs[i].NumberToSpawn; j++)
                {
                    SpawnEntity(entityPrefabs[i]);
                    yield return new WaitForSeconds(spawnDelay);
                }
            }

            yield return new WaitUntil(() => EnemyManager.Instance.ActiveEnemies.Count == 0);
            yield return new WaitForSeconds(EnemyManager.Instance.WaveTimer);
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
    public int TotalEnemiesToSpawnPerWave()
    {
        int totalEnemies = 0;

        for (int i = 0; i < entityPrefabs.Length; i++)
        {
            totalEnemies += entityPrefabs[i].NumberToSpawn;
        }

        return totalEnemies;
    }
}
