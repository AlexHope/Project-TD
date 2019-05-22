/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private float playerInitialHealth = 2000.0f;

    public int CurrentGold { get; set; }
    public int CurrentHealth { get; set; }
    public int TotalKills { get; set; }

    /// <summary>
    /// Assigns the static instance and sets the current health
    /// </summary>
    private void Awake()
    {
        Instance = this;
        CurrentHealth = (int)playerInitialHealth;
    }
}
