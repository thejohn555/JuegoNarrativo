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
        private Vector3 destinationPoint;
        [SerializeField]private float Velocity;
        
        public void SetNewDestination(Vector3 destinationPoint, float stoppingDistance)
        {
            StopAllCoroutines();
            this.stoppingDistance = stoppingDistance;
            this.destinationPoint = destinationPoint;
            currentDirection = (this.destinationPoint - transform.position).normalized;
            remainingDistance = (this.destinationPoint - transform.position).magnitude;
            StartCoroutine(MovePlayer());
        }

        private IEnumerator MovePlayer()
        {
            do
            {
                main.transform.position += currentDirection * Velocity; 
                remainingDistance = (destinationPoint - transform.position).magnitude;
                yield return new WaitForSeconds(0.01f);
            } while (!ReacheDestination());
        }

        public bool ReacheDestination()
        {
            return remainingDistance <= stoppingDistance;
        }
    }
}