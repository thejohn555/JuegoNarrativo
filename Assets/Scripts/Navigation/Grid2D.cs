using System;
using System.Collections.Generic;
using Managers;
using Navigation;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    [Serializable]
    public struct ScalePoint
    {
        [Min(0f)] public float offset;
        [Min(0f)] public float scale;
    }

    [Header("Cell Settings")]
    [Min(0.1f)]
    [SerializeField] private float cellSize = 0.5f;

    [Header("Floor Detection")]
    private Collider2D areaCollider;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Path Validation")]
    [Min(0.01f)]
    [SerializeField] private float lineCheckSpacing = 0.1f;

    [Header("Character Size")]
    [Min(0)]
    [SerializeField] private int walkableMargin = 1;
    
    [Header("Debug")]
    [SerializeField] private bool showGrid = true;

    [Header("Player Scale By Height")]
    [Tooltip("Offset medido desde el borde inferior del grid y multiplicador de escala.")]
    [SerializeField] private ScalePoint[] scalePoints = Array.Empty<ScalePoint>();

    private NodeGrid[,] grid;
    private readonly List<Collider2D> floorColliders = new List<Collider2D>();

    private int gridSizeX;
    private int gridSizeY;
    private Vector2 gridBottomLeft;
    private Vector2 gridWorldSize;

    public NodeGrid[,] Grid => grid;

    public float GetScaleAtWorldY(float worldY)
    {
        if (scalePoints == null || scalePoints.Length == 0)
            return 1f;

        float offset = worldY - GetGridBottomY();
        int lowerIndex = -1;
        int upperIndex = -1;

        for (int i = 0; i < scalePoints.Length; i++)
        {
            if (lowerIndex < 0 || scalePoints[i].offset < scalePoints[lowerIndex].offset)
                lowerIndex = i;
            if (upperIndex < 0 || scalePoints[i].offset > scalePoints[upperIndex].offset)
                upperIndex = i;
        }

        if (offset <= scalePoints[lowerIndex].offset)
            return scalePoints[lowerIndex].scale;
        if (offset >= scalePoints[upperIndex].offset)
            return scalePoints[upperIndex].scale;

        int beforeIndex = lowerIndex;
        int afterIndex = upperIndex;
        for (int i = 0; i < scalePoints.Length; i++)
        {
            float pointOffset = scalePoints[i].offset;
            if (pointOffset <= offset && pointOffset > scalePoints[beforeIndex].offset)
                beforeIndex = i;
            if (pointOffset >= offset && pointOffset < scalePoints[afterIndex].offset)
                afterIndex = i;
        }

        ScalePoint before = scalePoints[beforeIndex];
        ScalePoint after = scalePoints[afterIndex];
        if (Mathf.Approximately(before.offset, after.offset))
            return after.scale;

        float t = Mathf.InverseLerp(before.offset, after.offset, offset);
        return Mathf.Lerp(before.scale, after.scale, t);
    }

    private float GetGridBottomY()
    {
        if (areaCollider == null)
            areaCollider = GetComponent<Collider2D>();
        return areaCollider != null ? areaCollider.bounds.min.y : gridBottomLeft.y;
    }

    private void Awake()
    {
        areaCollider = GetComponent<Collider2D>();
        if (areaCollider == null)
        {
            Debug.LogError("Grid2D needs a Collider2D on the same GameObject.", this);
            return;
        }

        NavigationManager.Instance.InitializeGrid(this);
    }

    public void CreateGrid()
    {
        if (areaCollider == null)
        {
            Debug.LogError("Grid2D necesita un collider principal asignado en Area Collider.", this);
            return;
        }

        Bounds colliderBounds = areaCollider.bounds;
        gridWorldSize = colliderBounds.size;
        gridBottomLeft = colliderBounds.min;

        gridSizeX = Mathf.Max(
            1,
            Mathf.CeilToInt(gridWorldSize.x / cellSize)
        );

        gridSizeY = Mathf.Max(
            1,
            Mathf.CeilToInt(gridWorldSize.y / cellSize)
        );

        grid = new NodeGrid[gridSizeX, gridSizeY];

        floorColliders.Clear();
        if (areaCollider != null && areaCollider.enabled)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(floorMask);
            filter.useTriggers = true;
            areaCollider.Overlap(filter, floorColliders);
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = gridBottomLeft
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

        ApplyWalkableMargin();

        int walkableCount = 0;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (grid[x, y].Walkable)
                    walkableCount++;
            }
        }

        Debug.Log(
            $"PRUEBA GRID: {gridSizeX}x{gridSizeY}, " +
            $"celdas transitables={walkableCount}, " +
            $"suelos detectados={floorColliders.Count}",
            this
        );
    }
    
    
    private void ApplyWalkableMargin()
    {
        if (walkableMargin <= 0)
            return;

        // Guardamos el estado original para evitar
        // que las modificaciones se afecten entre sí.
        bool[,] originalWalkable = new bool[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                originalWalkable[x, y] = grid[x, y].Walkable;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (!originalWalkable[x, y])
                    continue;

                // Comprobamos las celdas alrededor.
                for (int offsetX = -walkableMargin;
                     offsetX <= walkableMargin;
                     offsetX++)
                {
                    for (int offsetY = -walkableMargin;
                         offsetY <= walkableMargin;
                         offsetY++)
                    {
                        int checkX = x + offsetX;
                        int checkY = y + offsetY;

                        // Si el margen sale del grid,
                        // esta celda no tiene espacio suficiente.
                        if (checkX < 0 || checkX >= gridSizeX ||
                            checkY < 0 || checkY >= gridSizeY)
                        {
                            grid[x, y].Walkable = false;
                            break;
                        }

                        // Si una celda dentro del margen no es
                        // caminable, descartamos la celda actual.
                        if (!originalWalkable[checkX, checkY])
                        {
                            grid[x, y].Walkable = false;
                            break;
                        }
                    }

                    if (!grid[x, y].Walkable)
                        break;
                }
            }
        }
    }


    private bool IsCellInsideFloor(Vector2 center)
    {
        if (areaCollider == null || !areaCollider.OverlapPoint(center)
            || Physics2D.OverlapPoint(center, obstacleMask) != null)
            return false;

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
                if (!areaCollider.OverlapPoint(point)
                    || Physics2D.OverlapPoint(point, obstacleMask) != null
                    || !floor.OverlapPoint(point))
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
        if (areaCollider == null || !areaCollider.OverlapPoint(worldPosition)
            || Physics2D.OverlapPoint(worldPosition, obstacleMask) != null)
            return false;

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

    
    
    public bool CanWalkThroughGrid(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        float spacing = cellSize * 0.2f;
        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(distance / spacing)
        );

        for (int i = 0; i <= steps; i++)
        {
            Vector2 point = Vector2.Lerp(
                start,
                end,
                (float)i / steps
            );

            NodeGrid node = NodeFromWorldPosition(point);

            if (!node.Walkable)
                return false;
        }

        return true;
    }


    
    public NodeGrid NodeFromWorldPosition(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt(
            (worldPosition.x - gridBottomLeft.x) / cellSize
        );

        int y = Mathf.FloorToInt(
            (worldPosition.y - gridBottomLeft.y) / cellSize
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
        if (areaCollider == null)
            areaCollider = GetComponent<Collider2D>();
        Bounds bounds = areaCollider != null
            ? areaCollider.bounds
            : new Bounds(transform.position, Vector3.zero);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (scalePoints != null)
        {
            Gizmos.color = Color.magenta;
            foreach (ScalePoint point in scalePoints)
            {
                float y = bounds.min.y + point.offset;
                Gizmos.DrawLine(
                    new Vector3(bounds.min.x, y, bounds.center.z),
                    new Vector3(bounds.max.x, y, bounds.center.z)
                );
            }
        }

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
