using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovementSystem : PlayerSystem
    {
        // private void OnClick()
        // {
        //     Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //     if(!Physics.Raycast(ray, out RaycastHit hit,whatIsClickable))return;
        //     
        //     attackSystem.ClearCurrentTarget();
        //     
        //     if (hit.collider.TryGetComponent(out Interactuable newinteractuable))
        //     {
        //         currentInteractuable = newinteractuable;
        //         currentStoppingDistance = newinteractuable.InteractionDistance;
        //         if (newinteractuable.TryGetComponent(out IDamageable damageable))
        //         {
        //             attackSystem.SetNewTarget(damageable);
        //         }
        //         StartCoroutine(TrackingInteractuable());
        //     }
        //     else
        //     {
        //         currentStoppingDistance = 0f;
        //         StopAllCoroutines();
        //         currentInteractuable = null;
        //         currentDestination = hit.point;
        //         playerNavMovementSystem.SetNewDestination(currentDestination,currentStoppingDistance);
        //     }
        // }
        //
        // private IEnumerator TrackingInteractuable()
        // {
        //     do
        //     {
        //         currentDestination = currentInteractuable.transform.position;
        //         playerNavMovementSystem.SetNewDestination(currentDestination,currentStoppingDistance);
        //         yield return new WaitForSeconds(0.2f);
        //     } while (!playerNavMovementSystem.ReacheDestination());
        //     currentInteractuable.Interact(gameObject);
        // }
    }
}