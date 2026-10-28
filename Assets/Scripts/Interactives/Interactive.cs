using Assets.Scripts.Camera;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interactives
{
    [RequireComponent(typeof(Outliner))]
    public abstract class Interactive:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [field: SerializeField] public float InteractionDistance { get; private set; } = 1.5f;
        [SerializeField] private bool showInteractionGizmo = true;
        public RuntimeDialogueGraph description;
        [SerializeField] protected Vector3 textOffsetAbovePlayer = new Vector3(0f, 2.5f, 0f);
        [SerializeField] protected Texture2D interactionCursor;
        protected Outliner outline;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            outline = GetComponent<Outliner>();
            outline.SwapOutline();
        }

        private void OnDrawGizmos()
        {
            if (!showInteractionGizmo) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, InteractionDistance);
        }

        public abstract void Interact(GameObject interactor);

        public void ShowDescription(GameObject interactor)
        {
            if (description == null || interactor == null) return;

            DialogueData monologueCameraData = new DialogueData
            {
                targetPosition = interactor.transform,
                cameraOffset = Vector3.zero,
                dialogueTextOffset = textOffsetAbovePlayer,
                zoom = Camera.main.orthographicSize,
                smoothTime = 0.2f,
                moveCamera = false,
                zoomCamera = false,
                isDialogue = false
            };
            Debug.Log($"Player Pos:{interactor.transform.position}");
            DialogueManager.Instance.StartDialogue(description, monologueCameraData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Cambio del cursor
            // Cursor.SetCursor(interactionCursor, new Vector2(0.5f,0.5f), CursorMode.Auto);
            outline.SwapOutline();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Cambio del cursor
            // Cursor.SetCursor(null, new Vector2(0.5f,0.5f), CursorMode.Auto);
            outline.SwapOutline();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            //Vector3 textPosition = transform.position + textOffsetAbovePlayer;

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(transform.position, textPosition);

            //Gizmos.DrawWireSphere(textPosition, 0.2f);

            //Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.35f);
            //Gizmos.DrawCube(textPosition, new Vector3(2f, 0.8f, 0.01f));
        }
    }
}
