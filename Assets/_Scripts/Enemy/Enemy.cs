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
    [SerializeField] protected EnemySize size;
    [SerializeField] protected float movementSpeed = 1.0f;

    [Header("Pathfinding")]
    [SerializeField] protected float pathCheckRate = 1.0f;
    protected Transform target;
    private Stack<PF_Node> currentPath = new Stack<PF_Node>();

    private int totalLifetime;

    /// <summary>
    /// Finds the target and starts the path resolver
    /// </summary>
    protected override void OnEnable()
    {
        if (EnemyManager.Instance)
        {
            target = EnemyManager.Instance.EnemyTarget;
        }

        StartCoroutine(PathResolver());
    }
	
	/// <summary>
    /// Performs enemy logic every frame
    /// </summary>
	protected override void Update()
    {
        if (IsActive)
        {
            PerformPathfinding();
        }
    }

    /// <summary>
    /// Runs after update to destroy this enemy if it's health has reached zero
    /// </summary>
    protected override void LateUpdate()
    {
        if (Health <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Removes this entity from the list of active enemies on destroy
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (EnemyManager.Instance && EnemyManager.Instance.ActiveEnemies.Contains(this))
        {
            EnemyManager.Instance.ActiveEnemies.Remove(this);
        }
    }

    /// <summary>
    /// Performs the pathfinding
    /// </summary>
    private void PerformPathfinding()
    {
        // Determine if we have a target
        if (target)
        {
            // If we do have a valid path to the target, move towards it
            if (currentPath != null && currentPath.Count > 0)
            {
                try
                {
                    // If the distance to the next node is less than the radius i.e. we're inside the tile, remove that node from the current path
                    if (Vector2.Distance(transform.position, currentPath.Peek().WorldPosition) < PF_Grid.Instance.NodeRadius)
                    {
                        currentPath.Pop();
                    }

                    // Move towards the next node in the path
                    transform.position = Vector2.MoveTowards(transform.position, currentPath.Peek().WorldPosition, 1.0f * movementSpeed);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(entityID + " failed to perform pathfinding: " + e.Message);
                }
            }
            // Otherwise destroy this
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Waits a specified number of seconds before determining the best path to the target
    /// </summary>
    private IEnumerator PathResolver()
    {
        currentPath = PF_Pathfinder.Instance.CalculatePath(transform, target);
        yield return new WaitForSeconds(pathCheckRate);
    }
}
