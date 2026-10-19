using System.Collections;
using Interactives;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteractSystem : PlayerSystem
    {
        [SerializeField]private float DefaultStoppingDistance;
        private float currentStoppingDistance;
        private Interactive currentInteractuable;
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
            
            Vector2  mousePosition  = main.Cam.ScreenToWorldPoint(
                Mouse.current.position.ReadValue());

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 
                Mathf.Infinity, main.whatIsClickable);
            
            if(!hit.collider)return;
            
            if (hit.collider.TryGetComponent(out Interactive newInteractuable))
            {
                currentInteractuable = newInteractuable;
                currentStoppingDistance = newInteractuable.InteractionDistance;
                StartCoroutine(TrackingInteractuable());
            }
            else
            {
                currentStoppingDistance = DefaultStoppingDistance;
                StopAllCoroutines();
                currentInteractuable = null;
                currentDestination = hit.point;
                main.MovementSystem.SetNewDestination(currentDestination,currentStoppingDistance);
            }
            
        }
        
        private IEnumerator TrackingInteractuable()
        {
            do
            {
                currentDestination = currentInteractuable.transform.position;
                main.MovementSystem.SetNewDestination(currentDestination,currentStoppingDistance);
                yield return new WaitForSeconds(0.2f);
            } while (!main.MovementSystem.ReacheDestination());
            currentInteractuable.Interact(gameObject);
        }
        
    }
}