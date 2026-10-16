using System.Collections;
using Interactives;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteractSystem : PlayerSystem
    {
        private Interactive currentInteractuable;
        private float currentStoppingDistance;
        private Vector3 currentDestination;
        
        private void OnEnable()
        {
            EventBus.Instance.Subscribe<OnLeftMouseClick>(Clicked);
        }

        private void OnDisable()
        {
            EventBus.Instance.UnSubscribe<OnLeftMouseClick>(Clicked);
        }
        
        private void Clicked(OnLeftMouseClick ctx)
        {
            Ray ray = main.cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(!Physics.Raycast(ray, out RaycastHit hit,main.whatIsClickable))return;
            
            if (hit.collider.TryGetComponent(out Interactive newInteractuable))
            {
                currentInteractuable = newInteractuable;
                currentStoppingDistance = newInteractuable.InteractionDistance;
                StartCoroutine(TrackingInteractuable());
            }
            else
            {
                currentStoppingDistance = 0f;
                StopAllCoroutines();
                currentInteractuable = null;
                currentDestination = hit.point;
                main.playerMovementSystem.SetNewDestination(currentDestination,currentStoppingDistance);
            }
        }
        
        private IEnumerator TrackingInteractuable()
        {
            do
            {
                currentDestination = currentInteractuable.transform.position;
                main.playerMovementSystem.SetNewDestination(currentDestination,currentStoppingDistance);
                yield return new WaitForSeconds(0.2f);
            } while (!main.playerMovementSystem.ReacheDestination());
            currentInteractuable.Interact(gameObject);
        }
    }
}