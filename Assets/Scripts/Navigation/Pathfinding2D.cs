using System.Collections.Generic;
using Navigation;
using UnityEngine;

public class Pathfinding2D : MonoBehaviour
{
    [Header("Path Test")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private bool showPath = true;

    private List<NodeGrid> currentPath;
    
    [SerializeField] private Grid2D grid;

    public List<NodeGrid> FindPathToInteractionRange(
        Vector2 startPosition,
        Vector2 targetPosition,
        float interactionDistance,
        out Vector2 reachablePosition)
    {
        reachablePosition = targetPosition;
        if (grid == null || grid.Grid == null)
            return null;

        List<NodeGrid> candidates = new List<NodeGrid>();
        NodeGrid[,] nodes = grid.Grid;
        for (int x = 0; x < nodes.GetLength(0); x++)
        for (int y = 0; y < nodes.GetLength(1); y++)
        {
            NodeGrid node = nodes[x, y];
            if (node.Walkable && Vector2.Distance(node.WorldPosition, targetPosition) <= interactionDistance)
                candidates.Add(node);
        }

        candidates.Sort((a, b) =>
            Vector2.Distance(a.WorldPosition, startPosition).CompareTo(
                Vector2.Distance(b.WorldPosition, startPosition)));

        foreach (NodeGrid candidate in candidates)
        {
            List<NodeGrid> path = FindPath(startPosition, candidate.WorldPosition);
            if (path == null || path.Count == 0)
                continue;

            reachablePosition = candidate.WorldPosition;
            return path;
        }

        return null;
    }

    public List<NodeGrid> FindPath(
        Vector2 startPosition,
        Vector2 targetPosition)
    {
        NodeGrid startNode = grid.NodeFromWorldPosition(startPosition);
        NodeGrid targetNode = grid.NodeFromWorldPosition(targetPosition);

        // No buscamos rutas hacia celdas no transitables.
        if (!startNode.Walkable || !targetNode.Walkable)
            return null;

        List<NodeGrid> openSet = new List<NodeGrid>();
        HashSet<NodeGrid> closedSet = new HashSet<NodeGrid>();

        Dictionary<NodeGrid, NodeGrid> cameFrom = new Dictionary<NodeGrid, NodeGrid>();
        Dictionary<NodeGrid, float> gCost = new Dictionary<NodeGrid, float>();

        openSet.Add(startNode);
        gCost[startNode] = 0f;

        while (openSet.Count > 0)
        {
            NodeGrid currentNode = openSet[0];

            // Buscamos la celda con el menor coste estimado.
            foreach (NodeGrid node in openSet)
            {
                if (GetFCost(node, gCost, targetNode)
                    < GetFCost(currentNode, gCost, targetNode))
                {
                    currentNode = node;
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Hemos llegado al destino.
            if (currentNode == targetNode)
            {
                List<NodeGrid> path = RetracePath(
                    cameFrom,
                    startNode,
                    targetNode
                );


                List<NodeGrid> smoothedPath = SmoothPath(
                    path,
                    startPosition
                );


                return smoothedPath;
            }

            foreach (NodeGrid neighbour in grid.GetNeighbours(currentNode))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                float tentativeGCost =
                    gCost[currentNode] +
                    Vector2.Distance(
                        currentNode.WorldPosition,
                        neighbour.WorldPosition
                    );

                if (!gCost.ContainsKey(neighbour) ||
                    tentativeGCost < gCost[neighbour])
                {
                    cameFrom[neighbour] = currentNode;
                    gCost[neighbour] = tentativeGCost;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // No se ha encontrado una ruta.
        return null;
    }

    private float GetFCost(
        NodeGrid node,
        Dictionary<NodeGrid, float> gCost,
        NodeGrid targetNode)
    {
        float g = gCost.TryGetValue(node, out float cost)
            ? cost
            : float.PositiveInfinity;

        float h = Vector2.Distance(
            node.WorldPosition,
            targetNode.WorldPosition
        );

        return g + h;
    }

    private List<NodeGrid> RetracePath(
        Dictionary<NodeGrid, NodeGrid> cameFrom,
        NodeGrid startNode,
        NodeGrid targetNode)
    {
        List<NodeGrid> path = new List<NodeGrid>();
        NodeGrid currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            if (!cameFrom.TryGetValue(currentNode, out NodeGrid previousNode))
                return null;

            currentNode = previousNode;
        }

        path.Reverse();
        return path;
    }
    
    private List<NodeGrid> SmoothPath(
        List<NodeGrid> path,
        Vector2 startPosition)
    {
        if (path == null || path.Count == 0)
            return path;

        List<NodeGrid> smoothedPath = new List<NodeGrid>();

        Vector2 currentPosition = startPosition;
        int currentIndex = 0;

        while (currentIndex < path.Count)
        {
            int furthestVisibleIndex = -1;

            // Buscamos desde el último nodo hacia el primero.
            for (int i = path.Count - 1; i >= currentIndex; i--)
            {
                if (grid.CanWalkThroughGrid(
                        currentPosition,
                        path[i].WorldPosition))
                {
                    furthestVisibleIndex = i;
                    break;
                }
            }

            // Si no encontramos un tramo visible, conservamos
            // el siguiente nodo de la ruta original.
            if (furthestVisibleIndex == -1)
                furthestVisibleIndex = currentIndex;

            NodeGrid selectedNode = path[furthestVisibleIndex];

            smoothedPath.Add(selectedNode);
            currentPosition = selectedNode.WorldPosition;
            currentIndex = furthestVisibleIndex + 1;
        }

        return smoothedPath;
    }

    private void Update()
    {
        if (startPoint == null || targetPoint == null)
            return;

        currentPath = FindPath(
            startPoint.position,
            targetPoint.position
        );
    }

    private void OnDrawGizmos()
    {
        if (!showPath || currentPath == null)
            return;

        Gizmos.color = Color.cyan;
        
        Vector3 previousPosition = startPoint != null
            ? startPoint.position
            : currentPath[0].WorldPosition;

        foreach (NodeGrid node in currentPath)
        {
            Gizmos.DrawLine(previousPosition, node.WorldPosition);
            Gizmos.DrawSphere(node.WorldPosition, 0.08f);

            previousPosition = node.WorldPosition;
        }

        foreach (NodeGrid node in currentPath)
        {
            Gizmos.DrawSphere(node.WorldPosition, 0.08f);
        }
    }
}
