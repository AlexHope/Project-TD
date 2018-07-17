/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Turret : Entity
{
    public enum Tier
    {
        Tier1,
        Tier2,
        Tier3
    }

    [Header("Turret")]
    [SerializeField] protected Tier turretTier = Tier.Tier1;
    [SerializeField] protected float health = 100.0f;
    [SerializeField] protected float cost = 0.0f;
    [SerializeField, ShowInInspector] protected Attack attackType;

    private int totalLifetime;
    private int totalKills;

	// Use this for initialization
	private void Start()
    {
		
	}
	
	// Update is called once per frame
	private void Update()
    {
		
	}
}
