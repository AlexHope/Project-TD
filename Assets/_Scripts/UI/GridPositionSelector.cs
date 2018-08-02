/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPositionSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject selectionMarkerPrefab;

    public bool SelectionModeActive { get; set; }

    private GameObject selectionMarker;

    /// <summary>
    /// Run every frame to update the position of the cursor
    /// </summary>
    private void Update()
    {
        if (SelectionModeActive)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0) && !selectionMarker)
                {
                    // Spawn the selection marker
                    selectionMarker = Instantiate(selectionMarkerPrefab, transform);
                }

                // Move it
                selectionMarker.transform.position = CalculateGridPosition();
            }
        }
        else
        {
            // If the selection marker still exists when we leave selection mode, destroy it
            if (selectionMarker)
            {
                Destroy(selectionMarker);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SelectionModeActive = !SelectionModeActive;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Break();
        }
    }

    /// <summary>
    /// Determines the grid position for the current cursor position
    /// </summary>
    private Vector2 CalculateGridPosition()
    {
        // First, get the world position of the cursor
        Vector2 worldPosition = new Vector2();

        #if UNITY_STANDALONE
        worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            worldPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
#endif

        // Then, determine the closest node in the grid to that position
        worldPosition = Pathfinding.PF_Grid.Instance.GetNodeFromWorldPosition(worldPosition).WorldPosition;

        return worldPosition;
    }
}
