/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>
    /// This class is used to run the logic for determining the optimum path to take to reach a destination
    /// </summary>
    public class PF_Pathfinder : MonoBehaviour
    {
        public static PF_Pathfinder Instance { get; private set; }
        private PF_Grid grid;

        /// <summary>
        /// Assigns the static instance
        /// </summary>
        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Assigns needed references
        /// </summary>
        private void Start()
        {
            grid = PF_Grid.Instance;
        }

        /// <summary>
        /// Determines the distance between node A and node B
        /// </summary>
        /// <param name="nodeA">The first node</param>
        /// <param name="nodeB">The second node</param>
        /// <returns>The distance between the two nodes</returns>
        private float DistanceToNode(PF_Node nodeA, PF_Node nodeB)
        {
            return Vector2.Distance(nodeA.WorldPosition, nodeB.WorldPosition);
        }

        /// <summary>
        /// Determines the shortest path from the start position to the end position
        /// </summary>
        /// <param name="startPosition">The start position to begin the path from</param>
        /// <param name="endPosition">The end position</param>
        /// <returns>The shortest path from start to end</returns>
        public List<PF_Node> CalculatePath(Vector2 startPosition, Vector2 endPosition)
        {
            // Determine the start/end nodes
            PF_Node startNode = grid.GetNodeFromWorldPosition(startPosition);
            PF_Node endNode = grid.GetNodeFromWorldPosition(endPosition);

            List<PF_Node> openSet = new List<PF_Node>();
            HashSet<PF_Node> closedSet = new HashSet<PF_Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PF_Node currentNode = openSet[0];

                // Determine the next node from the open set
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost ||
                        (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }
                
                // Remove the selected node from the open list and add it to the closed list as we no longer need to check it
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // If we're at our target, return the path
                if (currentNode == endNode)
                {
                    return CreatePath(startNode, currentNode);
                }

                foreach (PF_Node neighbour in currentNode.Neighbours)
                {
                    // Ignore the node if it's already in the closed set or is impassable
                    if (closedSet.Contains(neighbour) || neighbour.Type == PF_Node.NodeType.Impassable)
                    {
                        continue;
                    }

                    // Determine the new movement cost to go from the current node to the neighbour
                    float newMovementCost = currentNode.GCost + DistanceToNode(currentNode, neighbour);

                    // We don't care about nodes that are already in the open set or less than the current G cost of that neighbour
                    if (!openSet.Contains(neighbour) || newMovementCost < neighbour.GCost)
                    {
                        // Update the costs for the neighbour
                        neighbour.GCost = newMovementCost;
                        neighbour.HCost = DistanceToNode(neighbour, endNode);
                        neighbour.Parent = currentNode;

                        // If not already in the open set, add it
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determines the shortest path from the start position to the end position
        /// </summary>
        /// <param name="startNode">The start node to begin the path from</param>
        /// <param name="endNode">The end node</param>
        /// <returns>The shortest path from start to end</returns>
        public List<PF_Node> CalculatePath(Transform startPosition, Transform endPosition)
        {
            return CalculatePath(startPosition.position, endPosition.position);
        }

        /// <summary>
        /// This will route a path back to the start node from the end node
        /// </summary>
        /// <param name="startNode">The starting node to begin the path from</param>
        /// <param name="endNode">The end node of the path</param>
        /// <returns></returns>
        private List<PF_Node> CreatePath(PF_Node startNode, PF_Node endNode)
        {
            List<PF_Node> path = new List<PF_Node>();
            PF_Node currentNode = endNode;

            // Traverse back through the nodes, starting from the end node
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            // If we've determined a path, reverse it to get the start to end node path
            if (path.Count != 0) path.Reverse();

            return path;
        }
    }
}

