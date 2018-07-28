using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoal : Entity
{
    /// <summary>
    /// Sets this entity's health on start
    /// </summary>
    protected override void Start()
    {
        base.Start();
        Health = PlayerManager.Instance.CurrentHealth;
    }

    /// <summary>
    /// When health reaches zero, this will fire the "you lose" event
    /// </summary>
    protected override void LateUpdate()
    {
        if (Health <= 0.0f)
        {
            // You lose
        }
    }
}
