using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;
    public Image iconImage;

    private static GameObject draggedIconObject;
    private static int draggedFromIndex = -1;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (iconImage == null || !iconImage.enabled || iconImage.sprite == null) return;

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

        RectTransform rt = (RectTransform)draggedIconObject.transform;
        rt.sizeDelta = iconImage.rectTransform.rect.size;   // tamaño real, no sizeDelta
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

        MoveGhost(eventData);
        iconImage.enabled = false;
    }

    public void OnDrag(PointerEventData eventData) => MoveGhost(eventData);

    static void MoveGhost(PointerEventData eventData)
    {
        if (draggedIconObject == null) return;

        RectTransform canvasRect = (RectTransform)draggedIconObject.transform.parent;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector3 worldPos))
        {
            draggedIconObject.transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedIconObject != null)
        {
            Destroy(draggedIconObject);
            InventoryManager.Instance.UpdateUI(); // Restaura la UI original si se soltó fuera
        }
        draggedFromIndex = -1;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedFromIndex != -1 && draggedFromIndex != slotIndex)
        {
            InventoryManager.Instance.TryCraftOrMove(draggedFromIndex, slotIndex);
        }

        if (draggedIconObject != null)
        {
            Destroy(draggedIconObject);
            draggedFromIndex = -1;
        }
    }
}