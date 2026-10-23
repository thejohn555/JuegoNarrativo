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
                return RetracePath(cameFrom, startNode, targetNode);

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

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(
                currentPath[i].WorldPosition,
                currentPath[i + 1].WorldPosition
            );
        }

        foreach (NodeGrid node in currentPath)
        {
            Gizmos.DrawSphere(node.WorldPosition, 0.08f);
        }
    }
}
