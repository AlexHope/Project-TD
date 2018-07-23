/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Turret : Entity
{
    public enum Tier
    {
        Tier1,
        Tier2,
        Tier3
    }

    [Header("Turret")]
    [SerializeField] protected Tier turretTier = Tier.Tier1;
    [SerializeField] protected float cost = 0.0f;

    [Header("Attack")]
    [SerializeField] protected float fireRate = 1.0f;
    [SerializeField] protected float range = 10.0f;
    [SerializeField] protected int maximumTargets = 1;

    protected List<Entity> targets = new List<Entity>();
    protected float totalLifetime = 0.0f;
    protected int totalKills = 0;

    protected abstract void Attack(Entity target);
    protected abstract void UpdateAttack();

    /// <summary>
    /// Assigns event listeners and starts the attack resolver
    /// </summary>
    protected override void OnEnable()
    {
        // Listen to entity destruction events in case they were one of our targets
        OnEntityDestroyed += Turret_OnEntityDestroyed;

        // Start the attack resolver
        StartCoroutine(ResolveAttacks());
    }

    /// <summary>
    /// Updates the turret attacks
    /// </summary>
    protected override void Update()
    {
        base.Update();
        UpdateAttack();

        totalLifetime += Time.deltaTime;
    }

    /// <summary>
    /// Removes the destroyed entity from the target list if it contains it
    /// </summary>
    /// <param name="destroyedEntity">The destroyed entity</param>
    protected virtual void Turret_OnEntityDestroyed(Entity destroyedEntity)
    {
        if (targets.Contains(destroyedEntity))
        {
            targets.Remove(destroyedEntity);
            totalKills++;
        }
    }

    /// <summary>
    /// Runs the logic to attack an enemy
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResolveAttacks()
    {
        while (Application.isPlaying)
        {
            ScanForEnemies();

            // If we have any valid targets, fire at them
            if (targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Attack(targets[i]);
                }
                yield return new WaitForSeconds(fireRate);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Determines the closest (if any) target
    /// </summary>
    private void ScanForEnemies()
    {
        // Check which active enemies are in range
        if (EnemyManager.Instance)
        {
            targets.Clear();
            for (int i = 0; i < EnemyManager.Instance.ActiveEnemies.Count; i++)
            {
                // If we have enough targets, we don't need to keep checking
                if (targets.Count >= maximumTargets) break;

                // If the enemy is in range, add them to the list of targets
                if (Vector2.Distance(EnemyManager.Instance.ActiveEnemies[i].transform.position, transform.position) < range)
                {
                    targets.Add(EnemyManager.Instance.ActiveEnemies[i]);
                }
            }
        }
    }
}
