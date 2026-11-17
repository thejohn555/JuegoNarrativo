using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Solo dibuja: enlaza los slots del panel y pinta lo que le pasa el InventoryManager.
public class InventoryUI : MonoBehaviour
{
    private readonly List<Image> slotIconImages = new List<Image>();
    private readonly List<TextMeshProUGUI> slotIconNames = new List<TextMeshProUGUI>();
    
    // Vincula las referencias del panel que ya está dibujado en la escena
    public void Bind(GameObject panel)
    {
        slotIconImages.Clear();
        slotIconNames.Clear();

        foreach (Transform slotChild in panel.transform)
        {
            Transform iconTransform = slotChild.Find("Icon");
            if (iconTransform == null)
            {
                continue;
            }

            slotIconImages.Add(iconTransform.GetComponent<Image>());

            Transform textTransform = slotChild.Find("Text");
            TextMeshProUGUI text = textTransform != null ? textTransform.GetComponent<TextMeshProUGUI>() : null;
            slotIconNames.Add(text);

            if (text != null)
            {
                SlotTextMouseTracker hover = slotChild.GetComponent<SlotTextMouseTracker>();
                
                if (hover == null)
                {
                    hover = slotChild.gameObject.AddComponent<SlotTextMouseTracker>();
                }
                hover.Init(text);
            }
        }
    }
    
    public void Refresh(InventoryItem[] slots, int heldSlotIndex)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slotIconImages.Count; i++)
        {
            bool hasItem;

            if (i < slots.Length && !slots[i].IsEmpty && i != heldSlotIndex)
            {
                hasItem = true;
            }
            else
            {
                hasItem = false;
            }

            if (hasItem)
            {
                slotIconImages[i].sprite = slots[i].icon;
                slotIconImages[i].enabled = slots[i].icon != null;
            }
            else
            {
                slotIconImages[i].sprite = null;
                slotIconImages[i].enabled = false;
            }

            if (i < slotIconNames.Count && slotIconNames[i] != null)
            {
                if (hasItem)
                {
                    slotIconNames[i].text = slots[i].Name;
                }
                else
                {
                    slotIconNames[i].text = "";
                }
            }
        }
    }
}
