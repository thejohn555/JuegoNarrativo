using System.Collections.Generic;
using Navigation;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementSystem : PlayerSystem
{
    [Header("Movement")]
    [SerializeField] private float minVelocity = 1f;
    [FormerlySerializedAs("Velocity")]
    [SerializeField] private float maxVelocity = 3f;
    [SerializeField] private float distanceForMinVelocity = 3f;
    [SerializeField] private float distanceForMaxVelocity = 5f;

    [Header("Pathfinding")]
    [SerializeField] private Pathfinding2D pathfinding;

    private float stoppingDistance;
    private float currentVelocity;
    private Vector2 destinationPoint;

    private List<NodeGrid> currentPath;
    private int currentWaypointIndex;
    private bool isMoving;
    private bool destinationReached;

    public bool IsMoving => isMoving;
    public bool DestinationReached => destinationReached;

    public void SetNewDestination(
        Vector3 destination,
        float stoppingDistance,
        bool recalculateVelocity = true)
    {
        // Cancelamos la ruta anterior.
        isMoving = false;
        destinationReached = false;
        currentPath = null;

        this.stoppingDistance = stoppingDistance;
        destinationPoint = destination;

        if (recalculateVelocity)
        {
            float initialDistance = Vector2.Distance(
                transform.position,
                destinationPoint
            );
            currentVelocity = CalculateVelocity(initialDistance);
        }

        // Calculamos la ruta desde la posición actual.
        currentPath = pathfinding.FindPathToInteractionRange(
            transform.position,
            destinationPoint,
            stoppingDistance,
            out _
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

    private float CalculateVelocity(float distance)
    {
        if (distance <= distanceForMinVelocity)
            return minVelocity;

        if (distanceForMaxVelocity <= distanceForMinVelocity ||
            distance >= distanceForMaxVelocity)
            return maxVelocity;

        float speedFactor = Mathf.InverseLerp(
            distanceForMinVelocity,
            distanceForMaxVelocity,
            distance
        );

        return Mathf.Lerp(minVelocity, maxVelocity, speedFactor);
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
            FinishMovement(reached: true);
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
                FinishMovement(reached: true);
                return;
            }

            // El último nodo puede no coincidir con el clic exacto.
            // Por ahora, no continuamos fuera de la ruta calculada.
            FinishMovement(reached: false);
            return;
        }

        Vector2 target =
            currentPath[currentWaypointIndex].WorldPosition;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            target,
            this.currentVelocity * Time.deltaTime
        );

        main.transform.position = new Vector3(
            nextPosition.x,
            nextPosition.y,
            main.transform.position.z
        );
    }

    private void FinishMovement(bool reached)
    {
        isMoving = false;
        destinationReached = reached;
        currentPath = null;
    }

    public bool ReacheDestination()
    {
        return !isMoving;
    }
}
