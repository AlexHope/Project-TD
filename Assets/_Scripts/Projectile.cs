/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float BaseDamage { get; set; }
    public float BaseSpeed { get; set; }
    public Entity Owner { get; set; }
    public Entity Target { get; set; }

	/// <summary>
    /// Moves the projectile toward its target each frame
    /// </summary>
	private void Update()
    {
		if (Target)
        {
            transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, 1.0f * BaseSpeed);
        }
        else
        {
            // If we've lost our target, we need to deal with this projectile, so for now just destroy it
            // TODO: Make this less sudden
            Destroy(gameObject);
        }
	}

    /// <summary>
    /// Called when this projectile collides with something
    /// </summary>
    /// <param name="collision">The collision</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Determine if we've hit our target
        if (Target && collision.collider.gameObject == Target.gameObject)
        {
            // If we have, apply the damage and destroy this projectile
            ApplyDamage(Target);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Applies the damage to the target
    /// </summary>
    /// <param name="target">The target to damage</param>
    private void ApplyDamage(Entity target)
    {
        // If target was already at 0 health, ignore this call
        if (target.Health <= 0.0f) return;

        // Damage the target
        target.Health -= BaseDamage;
        if (target.Health <= 0.0f)
        {
            // If this projectile was fired by a tower, inform the tower they have killed their target
            if (Owner is Tower)
            {
                (Owner as Tower).DestroyedTarget(target);
            }
        }
    }

    /// <summary>
    /// Called when this projectile is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // If we were fired from a projectile tower, remove us from it's list of active projectiles
        ProjectileTower tower = Owner as ProjectileTower;
        if (tower && tower.ActiveProjectiles.Contains(gameObject))
        {
            tower.ActiveProjectiles.Remove(gameObject);
        }
    }
}
