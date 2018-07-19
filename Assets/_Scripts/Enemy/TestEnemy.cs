/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TestEnemy : Enemy
{
    [SerializeField] private Transform target;

	// Use this for initialization
	private void Start()
    {
		
	}
	
	// Update is called once per frame
	private void Update()
    {
        List<PF_Node> currentPath = PF_Pathfinder.Instance.CalculatePath(transform, target);

        if (currentPath.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentPath[0].WorldPosition, 0.2f);
        }
	}
}
