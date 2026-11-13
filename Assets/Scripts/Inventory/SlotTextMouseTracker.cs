using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTextMouseTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI nameText;

    public void Init(TextMeshProUGUI text)
    {
        nameText = text;
        SetVisible(false); // por defecto oculto
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetVisible(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetVisible(false);
    }

    // Si el inventario se cierra con el ratón encima, Unity no envía OnPointerExit
    private void OnDisable()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (nameText != null)
        {
            nameText.enabled = visible;
        }
    }
}