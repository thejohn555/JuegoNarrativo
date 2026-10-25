using System.Collections.Generic;
using Navigation;
using Player;
using UnityEngine;

public class PlayerMovementSystem : PlayerSystem
{
    [Header("Movement")]
    [SerializeField] private float Velocity = 3f;

    [Header("Pathfinding")]
    [SerializeField] private Pathfinding2D pathfinding;

    private float stoppingDistance;
    private Vector2 destinationPoint;

    private List<NodeGrid> currentPath;
    private int currentWaypointIndex;
    private bool isMoving;

    public bool IsMoving => isMoving;

    public void SetNewDestination(
        Vector3 destination,
        float stoppingDistance)
    {
        // Cancelamos la ruta anterior.
        isMoving = false;
        currentPath = null;

        this.stoppingDistance = stoppingDistance;
        destinationPoint = destination;

        // Calculamos la ruta desde la posición actual.
        currentPath = pathfinding.FindPath(
            transform.position,
            destinationPoint
        );

        // No existe una ruta válida.
        if (currentPath == null || currentPath.Count == 0)
        {
            isMoving = false;
            return;
        }

        currentWaypointIndex = 0;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving || currentPath == null)
            return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        Vector2 currentPosition = transform.position;

        // Si ya hemos llegado al destino, terminamos.
        if (Vector2.Distance(currentPosition, destinationPoint)
            <= stoppingDistance)
        {
            FinishMovement();
            return;
        }

        // Avanzamos por los puntos de la ruta.
        while (currentWaypointIndex < currentPath.Count)
        {
            Vector2 waypoint =
                currentPath[currentWaypointIndex].WorldPosition;

            if (Vector2.Distance(currentPosition, waypoint) > 0.05f)
                break;

            currentWaypointIndex++;
        }

        // Hemos recorrido todos los nodos.
        if (currentWaypointIndex >= currentPath.Count)
        {
            // Comprobamos si ya estamos suficientemente cerca.
            if (Vector2.Distance(currentPosition, destinationPoint)
                <= stoppingDistance)
            {
                FinishMovement();
                return;
            }

            // El último nodo puede no coincidir con el clic exacto.
            // Por ahora, no continuamos fuera de la ruta calculada.
            FinishMovement();
            return;
        }

        Vector2 target =
            currentPath[currentWaypointIndex].WorldPosition;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            target,
            Velocity * Time.deltaTime
        );

        main.transform.position = new Vector3(
            nextPosition.x,
            nextPosition.y,
            main.transform.position.z
        );
    }

    private void FinishMovement()
    {
        isMoving = false;
        currentPath = null;
    }

    public bool ReacheDestination()
    {
        return !isMoving;
    }
}