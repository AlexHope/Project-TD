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

    protected abstract void Attack(Entity target);
    protected abstract void UpdateAttack();

    public int TotalKills { get; set; }

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
            // Only scan for new targets if we are capable of finding a new target
            if (targets.Count < maximumTargets)
            {
                ScanForEnemies();
            }

            // If we have any valid targets, fire at them
            if (targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    // Determine if any of our current targets are out of range and re-assign them if they are
                    if (!targets[i] || Vector2.Distance(transform.position, targets[i].transform.position) > range)
                    {
                        targets[i] = FindClosestTarget();
                    }

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
        for (int i = 0; i < (maximumTargets - targets.Count); i++)
        {
            Entity closestTarget = FindClosestTarget();
            if (closestTarget)
            {
                targets.Add(closestTarget);
            }
        }
    }

    /// <summary>
    /// Finds the closest target within range of this turret
    /// </summary>
    /// <returns>The closest turret</returns>
    private Entity FindClosestTarget()
    {
        Entity closestTarget = null;

        if (EnemyManager.Instance)
        {
            float closestDistance = 9999.0f;

            // Check the active enemies
            for (int i = 0; i < EnemyManager.Instance.ActiveEnemies.Count; i++)
            {
                // Determine the distance to the enemy
                float distanceToTarget = Vector2.Distance(transform.position, EnemyManager.Instance.ActiveEnemies[i].transform.position);

                // If it's within range and closer than the previous closest entity, it is now the new closest target
                if (distanceToTarget < range && distanceToTarget < closestDistance && !targets.Contains(EnemyManager.Instance.ActiveEnemies[i]))
                {
                    closestDistance = distanceToTarget;
                    closestTarget = EnemyManager.Instance.ActiveEnemies[i];
                }
            }
        }

        return closestTarget;
    }
}
