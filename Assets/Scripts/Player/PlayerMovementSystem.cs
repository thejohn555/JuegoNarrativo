using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovementSystem : PlayerSystem
    {
        public void SetNewDestination(Vector3 destinationPoint, float stoppingDistance)
        {
            main.Agent.stoppingDistance = stoppingDistance;
            main.Agent.SetDestination(destinationPoint);
        }

        public bool ReacheDestination()
        {
            return !main.Agent.pathPending&&main.Agent.remainingDistance <= main.Agent.stoppingDistance;
        }
    }
}