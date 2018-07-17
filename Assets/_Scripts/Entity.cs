/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for all entities
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [Header("Base Entity")]
    public string Name;

    protected Guid entityID = Guid.NewGuid();
    private SpriteRenderer spriteRenderer;

    // Accessors
    public bool IsActive { get; set; }
    public Guid EntityID { get { return entityID; } }

	// Use this for initialization
	private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    /// <summary>
    /// Generates a new EntityID for the entity
    /// </summary>
    public void GenerateNewID()
    {
        entityID = Guid.NewGuid();
    }
}
