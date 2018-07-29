/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; set; }

    [SerializeField] private GameObject[] tier1TowerPrefabs;
    [SerializeField] private GameObject[] tier2TowerPrefabs;
    [SerializeField] private GameObject[] tier3TowerPrefabs;

    /// <summary>
    /// Assigns the static instance
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Finds the list of tower prefabs from a supplied tier
    /// </summary>
    /// <param name="tier">The tier of tower to return</param>
    /// <returns>An array of the tower prefabs for the specified tier</returns>
    public GameObject[] GetTowerPrefabsFromTier(Tower.Tier tier)
    {
        switch(tier)
        {
            case Tower.Tier.Tier1:
                {
                    return tier1TowerPrefabs;
                }
            case Tower.Tier.Tier2:
                {
                    return tier2TowerPrefabs;
                }
            case Tower.Tier.Tier3:
                {
                    return tier3TowerPrefabs;
                }
            default: goto case Tower.Tier.Tier1;
        }
    }

    /// <summary>
    /// Creates an instance of the passed in prefab
    /// </summary>
    /// <param name="towerPrefab">The tower to create</param>
    public void CreateTower(GameObject towerPrefab)
    {
        if (towerPrefab)
        {
            Debug.Log("Creating a " + towerPrefab.name + "...");
        }
    }
}
