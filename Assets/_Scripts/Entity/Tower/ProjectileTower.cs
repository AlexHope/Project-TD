/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : Tower
{
    [Header("Projectile Tower")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileDamage = 1.0f;
    [SerializeField] private float projectileSpeed = 1.0f;

    public List<GameObject> ActiveProjectiles { get; set; }

    /// <summary>
    /// Initialises the list of active projectiles
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        ActiveProjectiles = new List<GameObject>();
    }

    /// <summary>
    /// Spawns the projectile and adds it to the active projectiles list
    /// </summary>
    /// <param name="target"></param>
    protected override void Attack(Entity target)
    {
        // Create the projectile
        Projectile proj = GameObject.Instantiate(projectilePrefab, transform.position, transform.rotation, transform).GetComponent<Projectile>();
        proj.Owner = this;
        proj.Target = target;
        proj.BaseDamage = projectileDamage;
        proj.BaseSpeed = projectileSpeed;

        // Add it to the list of active projectiles
        ActiveProjectiles.Add(proj.gameObject);
    }

    /// <summary>
    /// Run every frame to update the attacks
    /// </summary>
    protected override void UpdateAttack()
    {
        // Left empty
    }
}
