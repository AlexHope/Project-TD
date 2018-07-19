/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// This class represents a node used by the AI for pathfinding
    /// </summary>
    public class PF_Node
    {
        public enum NodeType
        {
            Normal,
            Impassable
        }

        // Positions
        public Vector2 WorldPosition { get; private set; }
        public Vector2Int GridPosition { get; private set; }

        // Values required for the algorithm
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost { get { return GCost + HCost; } }
        public PF_Node Parent { get; set; }
        public List<PF_Node> Neighbours { get; private set; }

        // Variables of the node behaviour
        public NodeType Type { get; private set; }

        /// <summary>
        /// Default constructor for a node
        /// </summary>
        /// <param name="position">The world position of the node</param>
        /// <param name="gridPosition">The X/Y position in the grid of nodes</param>
        /// <param name="isTraversable">Whether the node should be counted as viable for pathing</param>
        public PF_Node(Vector2 position, Vector2Int gridPosition, NodeType type)
        {
            WorldPosition = position;
            GridPosition = gridPosition;
            Type = type;
        }

        /// <summary>
        /// Initialises the node, called by the grid on start
        /// </summary>
        /// <param name="allowDiagonal">Whether to allow diagonal neighbours</param>
        public void Initialise(bool allowDiagonal)
        {
            SetNeighbours(allowDiagonal);
        }

        /// <summary>
        /// Determines the neighbouring nodes to this node
        /// </summary>
        /// <param name="allowDiagonal">Whether to allow diagonal neighbours</param>
        private void SetNeighbours(bool allowDiagonal)
        {
            Neighbours = new List<PF_Node>();

            // Assign each neighbour
            SetNeighbour(1, 0);     // Right
            SetNeighbour(-1, 0);    // Left
            SetNeighbour(0, 1);     // Top
            SetNeighbour(0, -1);    // Bottom

            if (allowDiagonal)
            {
                SetNeighbour(1, 1);     // Top-Right
                SetNeighbour(-1, 1);    // Top-Left
                SetNeighbour(1, -1);     // Bottom-Left
                SetNeighbour(-1, -1);    // Bottom-Right
            }
        }

        /// <summary>
        /// Adds the neighbouring node to the list of neighbours
        /// </summary>
        /// <param name="xOffset">The X offset from this node's position in the grid</param>
        /// <param name="yOffset">The Y offset from this node's position in the grid</param>
        private void SetNeighbour(int xOffset, int yOffset)
        {
            int x = GridPosition.x + xOffset;
            int y = GridPosition.y + yOffset;
            if (x >= 0 && x < PF_Grid.Instance.Size.x)
            {
                if (y >= 0 && y < PF_Grid.Instance.Size.y)
                {
                    Neighbours.Add(PF_Grid.Instance.Nodes[x, y]);
                }
            }
        }
    }
}

