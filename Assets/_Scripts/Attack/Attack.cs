/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Attack
{
    public enum AttackType
    {
        Projectile,
        Laser
    }

    [Header("Attack")]
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float damage = 1.0f;
    [SerializeField] protected float attackRate = 1.0f;
    [SerializeField] protected float lifespan = -1;
    [SerializeField] protected float speed = 1.0f;

    /// <summary>
    /// Method that will attack the specified target
    /// </summary>
    /// <param name="target">The target to attack</param>
    public abstract void AttackTarget(GameObject target);
}
