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
            Destroy(gameObject);
        }
	}

    /// <summary>
    /// Called when this projectile collides with something
    /// </summary>
    /// <param name="collision">The collision</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Target && collision.collider.gameObject == Target.gameObject)
        {
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
        target.Health -= BaseDamage;
        if (target.Health <= 0.0f)
        {
            // If this projectile was fired by a turret, increment the turret's kills
            if (Owner is Turret)
            {
                (Owner as Turret).TotalKills++;
            }
        }
    }

    /// <summary>
    /// Called when this projectile is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // If we were fired from a projectile turret, remove us from it's list of active projectiles
        ProjectileTurret turret = Owner as ProjectileTurret;
        if (turret && turret.ActiveProjectiles.Contains(gameObject))
        {
            turret.ActiveProjectiles.Remove(gameObject);
        }
    }
}
