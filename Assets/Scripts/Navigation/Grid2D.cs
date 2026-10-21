using System.Collections.Generic;
using Navigation;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    [Header("Grid Bounds")]
    [SerializeField] private Vector2 gridWorldSize = new Vector2(20f, 10f);

    [Header("Cell Settings")]
    [Min(0.1f)]
    [SerializeField] private float cellSize = 0.5f;

    [Header("Floor Detection")]
    [SerializeField] private LayerMask floorMask;

    [Header("Path Validation")]
    [Min(0.01f)]
    [SerializeField] private float lineCheckSpacing = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool showGrid = true;

    private NodeGrid[,] grid;
    private Collider2D[] floorColliders;

    private int gridSizeX;
    private int gridSizeY;

    public NodeGrid[,] Grid => grid;

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        gridSizeX = Mathf.Max(
            1,
            Mathf.CeilToInt(gridWorldSize.x / cellSize)
        );

        gridSizeY = Mathf.Max(
            1,
            Mathf.CeilToInt(gridWorldSize.y / cellSize)
        );

        grid = new NodeGrid[gridSizeX, gridSizeY];

        Collider2D[] allColliders =
            FindObjectsByType<Collider2D>(
                FindObjectsSortMode.None
            );

        List<Collider2D> validColliders =
            new List<Collider2D>();

        foreach (Collider2D col in allColliders)
        {
            int layerBit = 1 << col.gameObject.layer;

            if ((floorMask.value & layerBit) != 0
                && col.enabled
                && col.gameObject.activeInHierarchy)
            {
                validColliders.Add(col);
            }
        }

        floorColliders = validColliders.ToArray();

        Vector2 bottomLeft = (Vector2)transform.position
            - gridWorldSize / 2f;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = bottomLeft
                    + new Vector2(
                        x * cellSize + cellSize / 2f,
                        y * cellSize + cellSize / 2f
                    );

                bool walkable = IsCellInsideFloor(worldPoint);

                grid[x, y] = new NodeGrid(
                    worldPoint,
                    walkable,
                    x,
                    y
                );
            }
        }
    }

    private bool IsCellInsideFloor(Vector2 center)
    {
        float halfSize = cellSize / 2f;

        Vector2[] samplePoints =
        {
            center,
            center + new Vector2(-halfSize, -halfSize),
            center + new Vector2(halfSize, -halfSize),
            center + new Vector2(-halfSize, halfSize),
            center + new Vector2(halfSize, halfSize),
            center + Vector2.down * halfSize,
            center + Vector2.up * halfSize,
            center + Vector2.left * halfSize,
            center + Vector2.right * halfSize
        };

        foreach (Collider2D floor in floorColliders)
        {
            if (floor == null
                || !floor.enabled
                || !floor.gameObject.activeInHierarchy)
            {
                continue;
            }

            bool allPointsInside = true;

            foreach (Vector2 point in samplePoints)
            {
                if (!floor.OverlapPoint(point))
                {
                    allPointsInside = false;
                    break;
                }
            }

            if (allPointsInside)
                return true;
        }

        return false;
    }

    public bool IsPositionWalkable(Vector2 worldPosition)
    {
        foreach (Collider2D floor in floorColliders)
        {
            if (floor != null
                && floor.enabled
                && floor.gameObject.activeInHierarchy
                && floor.OverlapPoint(worldPosition))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanWalkStraight(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;

        float distance = direction.magnitude;

        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(distance / lineCheckSpacing)
        );

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;

            Vector2 point = Vector2.Lerp(start, end, t);

            if (!IsPositionWalkable(point))
                return false;
        }

        return true;
    }
    
    public NodeGrid NodeFromWorldPosition(Vector2 worldPosition)
    {
        Vector2 bottomLeft = (Vector2)transform.position
                             - gridWorldSize / 2f;

        int x = Mathf.FloorToInt(
            (worldPosition.x - bottomLeft.x) / cellSize
        );

        int y = Mathf.FloorToInt(
            (worldPosition.y - bottomLeft.y) / cellSize
        );

        // Evitamos salirnos de los límites de la cuadrícula.
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);

        return grid[x, y];
    }
    
    public List<NodeGrid> GetNeighbours(NodeGrid node)
    {
        List<NodeGrid> neighbours = new List<NodeGrid>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // No incluimos la propia celda.
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                // Comprobamos que la celda esté dentro de la cuadrícula.
                if (checkX < 0 || checkX >= gridSizeX ||
                    checkY < 0 || checkY >= gridSizeY)
                {
                    continue;
                }

                NodeGrid neighbour = grid[checkX, checkY];

                // Solo devolvemos celdas transitables.
                if (neighbour.Walkable)
                    neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                gridWorldSize.x,
                gridWorldSize.y,
                0f
            )
        );

        if (!showGrid || grid == null)
            return;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                NodeGrid node = grid[x, y];

                Gizmos.color = node.Walkable
                    ? Color.green
                    : Color.red;

                Gizmos.DrawCube(
                    node.WorldPosition,
                    Vector3.one * cellSize * 0.9f
                );
            }
        }
    }
}