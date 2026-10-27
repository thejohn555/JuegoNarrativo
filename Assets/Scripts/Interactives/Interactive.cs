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
        public RuntimeDialogueGraph description;
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

        public abstract void Interact(GameObject interactor);

        public void ShowDescription(GameObject interactor)
        {
            
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
    }
}
