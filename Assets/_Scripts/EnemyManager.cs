/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class EnemyManager : MonoBehaviour
{
    // Events
    public delegate void OnWaveStartEvent(int waveNumber);
    public delegate void OnWaveEndEvent(int waveNumber);
    public static event OnWaveStartEvent OnWaveStart;
    public static event OnWaveEndEvent OnWaveEnd;

    public static EnemyManager Instance { get; private set; }

    public EnemyGoal EnemyGoal;
    public float WaveTimer = 30.0f;

    // Accessors
    public List<Enemy> ActiveEnemies { get; set; }

    // Wave information
    public bool IsWaveActive { get; private set; }
    public int TotalEnemiesInWave { get; private set; }
    public int RemainingEnemiesInWave { get; set; }
    public int WaveNumber { get; private set; }
    public int DisplayWaveNumber { get { return WaveNumber + 1; } }

    /// <summary>
    /// Assigns the static instance
    /// </summary>
    private void Awake()
    {
        Instance = this;
        WaveNumber = -1;
        ActiveEnemies = new List<Enemy>();
    }

    /// <summary>
    /// Starts the wave timer & assigns listeners
    /// </summary>
    private void Start()
    {
        StartCoroutine(WaveStarter());

        // Ensure this is listener to when entities are destroyed so that enemy information can be updated
        Entity.OnEntityDestroyed += Entity_OnEntityDestroyed;
    }

    /// <summary>
    /// Runs logic every frame to control the enemies
    /// </summary>
    private IEnumerator WaveStarter()
    {
        yield return new WaitForSeconds(1.0f);
        while(Application.isPlaying)
        {
            // Update the wave information
            WaveNumber++;
            CalculateTotalEnemiesInWave();
            RemainingEnemiesInWave = TotalEnemiesInWave;

            // Update the wave state
            IsWaveActive = true;

            // Send the wave start event to all listeners
            if (OnWaveStart != null)
            {
                OnWaveStart.Invoke(WaveNumber);
            }

            Debug.Log("Started wave " + WaveNumber + "...");

            // Wait for all the active enemies to be destroyed before counting the wave as 'ended'
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => ActiveEnemies.Count == 0);

            // Update the wave state
            IsWaveActive = false;

            // Send the wave end event to all listeners
            if (OnWaveEnd != null)
            {
                OnWaveEnd.Invoke(WaveNumber);
            }

            Debug.Log("Wave " + WaveNumber + " ended...");

            // Wait before spawning the new wave
            yield return new WaitForSeconds(WaveTimer);
        }
    }

    /// <summary>
    /// Calculates the total number of enemies in the wave
    /// </summary>
    public void CalculateTotalEnemiesInWave()
    {
        TotalEnemiesInWave = 0;

        // Find all the active spawners, and tally up the total number of enemies each one will spawn
        Spawner[] spawners = FindObjectsOfType<Spawner>();
        for (int i = 0; i < spawners.Length; i++)
        {
            TotalEnemiesInWave += spawners[i].TotalEnemiesToSpawn();
        }
    }

    /// <summary>
    /// Updates relevant information when an entity is destroyed
    /// </summary>
    /// <param name="destroyedEntity">The destroyed entity</param>
    private void Entity_OnEntityDestroyed(Entity destroyedEntity)
    {
        // First, determine if it was an enemy that was destroyed
        Enemy enemy = destroyedEntity as Enemy;
        if (enemy)
        {
            // If it was, update relevant information
            if (ActiveEnemies.Contains(enemy))
            {
                ActiveEnemies.Remove(enemy);
                RemainingEnemiesInWave--;
            }
        }
    }
}
