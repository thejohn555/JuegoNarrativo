using System.Collections;
using Assets.Scripts.EventBuses;
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
            main.EventBus.Subscribe<OnLeftMouseClick>(LeftMouseClicked);
            main.EventBus.Subscribe<OnRightMouseClick>(RightMouseClicked);
        }

        private void OnDisable()
        {
            main.EventBus.UnSubscribe<OnLeftMouseClick>(LeftMouseClicked);
            main.EventBus.UnSubscribe<OnRightMouseClick>(RightMouseClicked);
        }

        private void RightMouseClicked()
        {
            if (!TryGetClickedInteractive(out Interactive interactable) ||
                interactable.description == null)
                return;

            StartTracking(interactable, showDescriptionOnArrival: true);
        }

        private bool showDescriptionOnArrival;

        private bool TryGetClickedInteractive(out Interactive interactable)
        {
            interactable = null;
            if (Mouse.current == null || main.Cam == null)
                return false;

            Vector2 mousePosition = main.Cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(
                mousePosition,
                Vector2.zero,
                Mathf.Infinity,
                main.whatIsClickable);

            return hit.collider && hit.transform.root.TryGetComponent(out interactable);
        }

        private void StartTracking(Interactive interactable, bool showDescriptionOnArrival)
        {
            StopAllCoroutines();
            currentInteractuable = interactable;
            currentStoppingDistance = interactable.InteractionDistance;
            this.showDescriptionOnArrival = showDescriptionOnArrival;
            StartCoroutine(TrackingInteractuable());
        }

        private void LeftMouseClicked()
        {
            Vector2 mousePosition = main.Cam.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            RaycastHit2D hit = Physics2D.Raycast(
                mousePosition,
                Vector2.zero,
                Mathf.Infinity,
                main.whatIsClickable
            );

            if (!hit.collider)
                return;
            
            if (hit.transform.root.TryGetComponent(out Interactive interactuable))
            {
                StartTracking(interactuable, showDescriptionOnArrival: false);
            }
            else
            {
                StopAllCoroutines();

                currentInteractuable = null;
                showDescriptionOnArrival = false;
                currentStoppingDistance = DefaultStoppingDistance;
                currentDestination = hit.point;

                    main.MovementSystem.SetNewDestination(
                    currentDestination,
                    currentStoppingDistance
                );
            }
        }
        
        private IEnumerator TrackingInteractuable()
        {
            Interactive target = currentInteractuable;
            if (target == null) yield break;
            bool isFirstDestination = true;
            do
            {
                if (target == null) yield break;

                currentDestination = target.transform.position;
                main.MovementSystem.SetNewDestination(
                    currentDestination,
                    currentStoppingDistance,
                    recalculateVelocity: isFirstDestination
                );
                isFirstDestination = false;
                yield return new WaitForSeconds(0.2f);
            } while (!main.MovementSystem.ReacheDestination());

            if (target == null) yield break;

            if (showDescriptionOnArrival)
            {
                target.ShowDescription(gameObject);
            }
            else
            {
                target.Interact(gameObject);
            }
        }
        
    }
}
