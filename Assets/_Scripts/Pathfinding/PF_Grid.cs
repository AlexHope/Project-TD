/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding
{
    /// <summary>
    /// This class represents the grid used to hold the list of possible nodes for the AI to use for pathfinding
    /// </summary>
    public class PF_Grid : MonoBehaviour
    {
        public static PF_Grid Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private Tilemap baseTilemap;
        [SerializeField] private LayerMask impassableLayers;
        [SerializeField] private float nodeRadius;
        [SerializeField] private float distance;

        [Header("Nodes")]
        [SerializeField] private bool allowDiagonalPathing;

        [Header("Debug")]
        [SerializeField] private bool debugMode;

        private float nodeDiameter;

        // Accessors
        public PF_Node[,] Nodes { get; private set; }
        public Vector2Int Size { get; private set; }

        /// <summary>
        /// Assigns the static instance
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Assigns the size of the node grid before creating & initialising the nodes
        /// </summary>
        private void Start()
        {
            // We need to determine the size of the node grid based on the world size
            nodeDiameter = nodeRadius * 2;

            // Set the grid size
            Size = new Vector2Int(  Mathf.RoundToInt(baseTilemap.size.x / nodeDiameter),
                                    Mathf.RoundToInt(baseTilemap.size.y / nodeDiameter));

            // Finally, create the grid of nodes to be used for pathfinding and initialise them
            CreateNodeGrid();
            foreach (PF_Node node in Nodes)
            {
                node.Initialise(allowDiagonalPathing);
            }
	    }

        /// <summary>
        /// Used to show a representation of the node grid
        /// </summary>
        private void OnDrawGizmos()
        {
            if (debugMode && Nodes != null)
            {
                Vector3 offset = new Vector3(Size.x / 2 + nodeRadius, Size.y / 2);

                Gizmos.DrawWireCube(transform.position + offset, new Vector3(baseTilemap.size.x, baseTilemap.size.y, 1));
                foreach (PF_Node node in Nodes)
                {
                    switch(node.Type)
                    {
                        case PF_Node.NodeType.Normal:
                            {
                                Gizmos.color = new Color(1, 1, 1, 0.1f);
                                break;
                            }
                        case PF_Node.NodeType.Impassable:
                            {
                                Gizmos.color = Color.red;
                                break;
                            }
                    }

                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - distance));
                }
            }
        }

        /// <summary>
        /// Used to create the list of nodes
        /// </summary>
        private void CreateNodeGrid()
        {
            // Create our array of nodes
            Nodes = new PF_Node[Size.x, Size.y];
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    // Calculate the world position of the node
                    Vector3 nodeWorldPosition = new Vector3(x + nodeRadius, y + nodeRadius, 0) * nodeDiameter;

                    // Determine if we're an impassable node or not
                    bool isImpassable = Physics2D.OverlapCircle(nodeWorldPosition, nodeRadius, impassableLayers);
                    PF_Node.NodeType nodeType = (isImpassable) ? PF_Node.NodeType.Impassable : PF_Node.NodeType.Normal;

                    // Add the node to the grid
                    Nodes[x, y] = new PF_Node(nodeWorldPosition, new Vector2Int(x, y), nodeType);
                }
            }
        }

        /// <summary>
        /// Attempts to get a node from a supplied position
        /// </summary>
        /// <param name="position">The world position to check</param>
        /// <returns>The found node</returns>
        public PF_Node GetNodeFromWorldPosition(Vector2 position)
        {
            float xRatio = Mathf.Clamp01(position.x / Size.x);
            float yRatio = Mathf.Clamp01(position.y / Size.y);

            int xPosition = Mathf.RoundToInt((Size.x - 1) * xRatio);
            int yPosition = Mathf.RoundToInt((Size.y - 1) * yRatio);

            return Nodes[xPosition, yPosition];
        }
    }
}

