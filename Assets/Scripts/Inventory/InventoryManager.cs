using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
   public static  InventoryManager Instance { get; set; }
   
   [Header("Settings")]
   [SerializeField] public InventorySettings settings;

   private struct InventoryItem
   {
      public string itemName;
      public Sprite icon;
   }

   [SerializeField] private InventoryItem[] inventorySlots;
   private Transform panelTransform;
   private List<Image> slotIconImages = new List<Image>();

   private void Awake()
   {
      if (Instance = null)
      {
         Instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }

   public void InitializeInventory()
   {
      Debug.Log("Se ha creado el inventario manager.");
      if (settings == null)
      {
         return;
      }

      inventorySlots = new InventoryItem[settings.rows * settings.columns];
      GameObject panel = InventoryBuilder.Build(settings);
      panelTransform = panel.transform;

      slotIconImages.Clear();
      foreach (Transform slotChild in panelTransform)
      {
         Transform iconTransform = slotChild.Find("Icon");
         if (iconTransform != null)
         {
            slotIconImages.Add(iconTransform.GetComponent<Image>());
         }
      }

      UpdateUI();
   }
   
   public bool AddItem(string itemName, Sprite icon)
   {
      for (int i = 0; i < inventorySlots.Length; i++)
      {
         if (inventorySlots[i].icon == null)
         {
            inventorySlots[i] = new InventoryItem { itemName = itemName, icon = icon };
            UpdateUI();
            return true;
         }
      }
      Debug.LogWarning("¡The inventory is full!");
      return false;
   }

   public void UpdateUI()
   {
      for (int i = 0; i < slotIconImages.Count; i++)
      {
         if (i < inventorySlots.Length && inventorySlots[i].icon != null)
         {
            slotIconImages[i].sprite = inventorySlots[i].icon;
            slotIconImages[i].enabled = true;
         }
         else
         {
            slotIconImages[i].sprite = null;
            slotIconImages[i].enabled = false;
         }
      }
   }
}
