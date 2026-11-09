using Assets.Scripts.Camera;
using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interactives
{
    [RequireComponent(typeof(Outliner))]
    public abstract class Interactive:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [field: SerializeField] public float InteractionDistance { get; private set; } = 1.5f;
        [field: SerializeField] public Vector2 VisualOffset { get; private set; } = Vector2.zero;
        [SerializeField] private bool showInteractionGizmo = true;
        public RuntimeDialogueGraph description;
        [SerializeField] protected Texture2D interactionCursor;
        protected Outliner outline;
        public bool IsInteractable { get; private set; } = true;
        protected virtual void Awake()
        {
        }

        protected virtual void OnEnable()
        {
            ItemInteractManager.Instance.PublishItem(this);
        }

        protected virtual void Start()
        {
            outline = GetComponent<Outliner>();
            outline.SwapOutline();
        }

        protected virtual void OnDisable()
        {
            ItemInteractManager.Instance.UnPublishItem(this);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!showInteractionGizmo) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, InteractionDistance);
        }

        public abstract void Interact(GameObject interactor);
        public void TryInteract(GameObject interactor)//Nuevo
        {
            if (!IsInteractable) return;
            Interact(interactor);
        }

        public void ShowDescription(GameObject interactor)
        {
            if (description == null || interactor == null) return;

            DialogueData monologueCameraData = new DialogueData
            {
                targetPosition = interactor.transform,
                cameraOffset = Vector3.zero,
                zoom = Camera.main.orthographicSize,
                smoothTime = 0.2f,
                moveCamera = false,
                zoomCamera = false,
                isDialogue = false,
                speakers = new Dictionary<string, SpeakerData>
                {
                    { "Conan", new SpeakerData { transform = interactor.transform, textColor = Color.white } }
                }
            };

            DialogueManager.Instance.StartDialogue(description, monologueCameraData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Cambio del cursor
            if (!IsInteractable) return;
            Cursor.SetCursor(interactionCursor, new Vector2(0.5f, 0.5f), CursorMode.Auto);
            UseOutliner(false);
        }

        public void UseOutliner(bool active)
        {
            outline.SetInteractionHighlight(active);
            outline.SwapOutline();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Cambio del cursor
            if (!IsInteractable) return;
            Cursor.SetCursor(null, new Vector2(0.5f,0.5f), CursorMode.Auto);
            UseOutliner(false);
        }
        public void SetInteractable(bool value)//Nuevo hecho por Nacho
        {
            Debug.Log($"[Interactive] {name} enabled = {value}");
            IsInteractable = value;
            if (!value)
                Cursor.SetCursor(null, new Vector2(0.5f, 0.5f), CursorMode.Auto);
        }
        
    }
}
