using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoal : Entity
{
    /// <summary>
    /// When health reaches zero, this will fire the "you lose" event
    /// </summary>
    private void LateUpdate()
    {
        if (Health <= 0.0f)
        {
            // You lose
        }
    }
}
