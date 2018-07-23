using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float baseDamage = 100.0f;
    [SerializeField] private float baseSpeed = 1.0f;

    public float BaseDamage { get { return baseDamage; } }
    public float BaseSpeed { get { return baseSpeed; } }
    public Entity Owner { get; set; }
    public Entity Target { get; set; }

	/// <summary>
    /// Moves the projectile toward its target each frame
    /// </summary>
	private void Update()
    {
		if (Target)
        {
            transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, 1.0f * baseSpeed);
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
        if (collision.collider.gameObject == Target.gameObject)
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
        target.Health -= baseDamage;
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
