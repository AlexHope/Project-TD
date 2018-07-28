/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the base class for all entities
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Entity : MonoBehaviour
{
    // Events
    public delegate void OnEntityDestroyedEvent(Entity destroyedEntity);
    public static event OnEntityDestroyedEvent OnEntityDestroyed;

    [Header("Base Entity")]
    public string DisplayName = "";
    public float Health = 100.0f;

    protected Guid entityID = Guid.NewGuid();
    protected SpriteRenderer spriteRenderer;

    // Accessors
    public bool IsActive { get; set; }
    public Guid EntityID { get { return entityID; } }
    public float TotalLifetime { get; private set; }

    /// <summary>
    /// Assigns references
    /// </summary>
    protected virtual void Start()
    {
        IsActive = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    /// <summary>
    /// Run when this entity is enabled
    /// </summary>
    protected virtual void OnEnable()
    {

    }

    /// <summary>
    /// Runs logic every frame
    /// </summary>
    protected virtual void Update()
    {
        TotalLifetime += Time.deltaTime;
    }

    /// <summary>
    /// Runs logic every frame
    /// </summary>
    protected virtual void LateUpdate()
    {

    }

    /// <summary>
    /// Runs logic on destroy
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (OnEntityDestroyed != null)
        {
            OnEntityDestroyed.Invoke(this);
        }
    }

    /// <summary>
    /// Generates a new EntityID for the entity
    /// </summary>
    public void GenerateNewEntityID()
    {
        entityID = Guid.NewGuid();
    }

    /// <summary>
    /// Finds the entity with the provided ID
    /// </summary>
    /// <param name="id">The ID to search for</param>
    /// <returns>The found entity if it exists, otherwise null</returns>
    public static Entity FindEntityByID(Guid id)
    {
        Entity[] entities = FindObjectsOfType<Entity>();
        return entities.FirstOrDefault(entity => entity.EntityID == id);
    }
}
