/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public EnemyGoal EnemyGoal;
    
    public List<Enemy> ActiveEnemies { get; set; }

    /// <summary>
    /// Assigns the static instance
    /// </summary>
    private void Awake()
    {
        Instance = this;
        ActiveEnemies = new List<Enemy>();
    }

    /// <summary>
    /// Runs logic every frame to control the enemies
    /// </summary>
    private void LateUpdate()
    {

    }
}
