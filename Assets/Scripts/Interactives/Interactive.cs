using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactives
{
    public abstract class Interactive:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [field: SerializeField] public float InteractionDistance { get; private set; } = 2f;
        [SerializeField] protected Texture2D interactionCursor;

        public abstract void Interact(GameObject interactor);

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Cambio del cursor
            // Cursor.SetCursor(interactionCursor, new Vector2(0.5f,0.5f), CursorMode.Auto);
            // outline.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Cambio del cursor
            // Cursor.SetCursor(null, new Vector2(0.5f,0.5f), CursorMode.Auto);
            // outline.enabled = false;
        }
    }
}