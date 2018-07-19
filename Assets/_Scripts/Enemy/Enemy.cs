/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class Enemy : Entity
{
    public enum EnemySize
    {
        Small,
        Medium,
        Large,
        Huge
    }

    [Header("Enemy")]
    [SerializeField] protected float health = 100.0f;
    [SerializeField] protected EnemySize size;
    [SerializeField] protected float speed = 1.0f;

    private int totalLifetime;

    // Use this for initialization
    private void Start()
    {
		
	}
	
	// Update is called once per frame
	private void Update()
    {
		
	}
}
