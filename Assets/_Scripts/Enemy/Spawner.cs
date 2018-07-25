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
    [SerializeField] private float waveDelay = 10.0f;
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

            yield return new WaitForSeconds(waveDelay);
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
}
