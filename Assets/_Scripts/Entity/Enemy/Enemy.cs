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
    [SerializeField] protected float damage = 10.0f;
    [SerializeField] protected int goldReward = 10;

    [Header("Pathfinding")]
    [SerializeField] protected float pathCheckRate = 1.0f;
    protected EnemyGoal target;
    private Stack<PF_Node> currentPath = new Stack<PF_Node>();

    public int GoldReward { get { return goldReward; } }

    /// <summary>
    /// Finds the target and starts the path resolver
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        if (EnemyManager.Instance)
        {
            target = EnemyManager.Instance.EnemyGoal;
        }

        StartCoroutine(PathResolver());
    }
	
	/// <summary>
    /// Performs enemy logic every frame
    /// </summary>
	protected override void Update()
    {
        base.Update();
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
        base.LateUpdate();
        if (Health <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Used to check if we've hit our goal
    /// </summary>
    /// <param name="collision">The collision</param>
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // Determine if we've hit our target
        if (collision.collider.transform == target.transform)
        {
            // Damage the target
            PlayerManager.Instance.CurrentHealth -= (int)damage;

            // Destroy this enemy by setting its health to 0
            Health = 0.0f;
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

                        // If we have no more path to reach our target, we can't move to the next node so leave early
                        if (currentPath.Count == 0) return;
                    }

                    // Move towards the next node in the path
                    transform.position = Vector2.MoveTowards(transform.position, currentPath.Peek().WorldPosition, 1.0f * movementSpeed);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(entityID + " failed to perform pathfinding: " + e.Message);
                }
            }
            // Otherwise destroy this by setting its health to 0
            else
            {
                Health = 0.0f;
            }
        }
    }

    /// <summary>
    /// Waits a specified number of seconds before determining the best path to the target
    /// </summary>
    private IEnumerator PathResolver()
    {
        while(Application.isPlaying)
        {
            currentPath = PF_Pathfinder.Instance.CalculatePath(transform, target.transform);
            yield return new WaitForSeconds(pathCheckRate);
        }
    }
}
