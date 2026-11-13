using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    public int slotIndex;
    public Image iconImage;

    private static GameObject draggedIconObject;
    private static int draggedFromIndex = -1;
    
    // Si el slot se desactiva mientras se arrastra (por ejemplo, al cerrar el inventario),
    // Unity ya no le envía OnEndDrag, así que la limpieza se hace aquí.
    void OnDisable()
    {
        if (draggedFromIndex == slotIndex)
        {
            EndDragCleanup();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (iconImage == null || !iconImage.enabled || iconImage.sprite == null)
        {
            return;
        }

        draggedFromIndex = slotIndex;

        Canvas mainCanvas = InventoryManager.Instance.GetComponentInParent<Canvas>();
        
        if (mainCanvas == null)
        {
            mainCanvas = Object.FindFirstObjectByType<Canvas>();
        }

        if (mainCanvas == null)
        {
            return;
        }
        
        mainCanvas = mainCanvas.rootCanvas;

        draggedIconObject = new GameObject("DraggedIcon", typeof(RectTransform));
        draggedIconObject.layer = mainCanvas.gameObject.layer;
        draggedIconObject.transform.SetParent(mainCanvas.transform, false);
        draggedIconObject.transform.SetAsLastSibling();

        Image tempImg = draggedIconObject.AddComponent<Image>();
        tempImg.sprite = iconImage.sprite;
        tempImg.color = iconImage.color;
        tempImg.raycastTarget = false;
        tempImg.preserveAspect = true;
        
        MoveGhost(eventData);
        iconImage.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Si el inventario se cierra mientras estás arrastrando, cancelamos al vuelo
        if (InventoryManager.Instance != null && !InventoryManager.Instance.gameObject.activeInHierarchy)
        {
            EndDragCleanup();
            return;
        }

        MoveGhost(eventData);
    }

    static void MoveGhost(PointerEventData eventData)
    {
        if (draggedIconObject == null)
        {
            return;
        }
        
        RectTransform canvasRect = (RectTransform)draggedIconObject.transform.parent;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector3 worldPos))
        {
            draggedIconObject.transform.localScale = new Vector3(0.4f, 0.4f);
            draggedIconObject.transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragCleanup();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedFromIndex != -1 && draggedFromIndex != slotIndex)
        {
            InventoryManager.Instance.TryCraftOrMove(draggedFromIndex, slotIndex);
        }

        EndDragCleanup();
    }

    //Metodo para evitar que el objeto al arrastrarlo fuera del inventario quede bugeado en la escena
    private void EndDragCleanup()
    {
        if (draggedIconObject != null)
        {
            Destroy(draggedIconObject);
            draggedIconObject = null;
        }

        draggedFromIndex = -1;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UpdateUI();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"Slot clicked {slotIndex} | button: {eventData.button} | dragging: {eventData.dragging} ");

        if (eventData.button != PointerEventData.InputButton.Left || eventData.dragging)
        {
            return;
        }

        if (HeldItemController.Instance == null)
        {
            Debug.LogError("There is no HeldItemController in the scene.");
            return;
        }

        HeldItemController.Instance.Hold(slotIndex);
    }
}