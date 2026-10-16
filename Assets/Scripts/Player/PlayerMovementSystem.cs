using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovementSystem : PlayerSystem
    {
        private float stoppingDistance;
        private float remainingDistance;
        private Vector3 currentDirection;
        [SerializeField]private float Velocity;
        
        public void SetNewDestination(Vector3 destinationPoint, float stoppingDistance)
        {
            this.stoppingDistance = stoppingDistance;
            currentDirection = (destinationPoint - transform.position).normalized;
            StartCoroutine(MovePlayer());
        }

        private IEnumerator MovePlayer()
        {
            do
            {
                main.transform.position += currentDirection * Velocity;
                yield return new WaitForSeconds(0.1f);
            } while (!ReacheDestination());
        }

        public bool ReacheDestination()
        {
            return remainingDistance <= stoppingDistance;
        }
    }
}