using Assets.Scripts.EventBuses;
using EventBuses;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Inputs")]
    [SerializeField] private EventBusSO inputs;

    private bool isOpen = false;

    private void Start()
    {
        inputs.Subscribe<OnScrollWheelPressed>(ToggleInventory);
        //Si no asignamos el panel en el inspector, intentamos encontrarlo automáticamente
        if (inventoryPanel == null && InventoryManager.Instance != null && InventoryManager.Instance.settings != null)
        {
            string panelName = InventoryManager.Instance.settings.inventoryName;
            GameObject foundPanel = GameObject.Find(panelName);
            if (foundPanel != null)
            {
                inventoryPanel = foundPanel;
            }
        }

        // Al iniciar el juego el inventario está desactivado
        if (inventoryPanel != null)
        {
            isOpen = false;
            inventoryPanel.SetActive(false);
        }
        
        if(HeldItemController.Instance != null)
        {
            HeldItemController.Instance.OnItemHeld += HandleItemHeld;
        }
    }

    private void OnDestroy()
    {
        if (HeldItemController.Instance != null)
        {
            HeldItemController.Instance.OnItemHeld -= HandleItemHeld;
        }
    }

    private void HandleItemHeld(string itemName)
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        inputs.Publish(new OnInventoryCloseEvent()); // el SO vuelve al mapa GamePlay
    }

    public void ToggleInventory(OnScrollWheelPressed eventArgs)
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            HeldItemController.Instance.Release(); // al abrir, el objeto sale de la mano
        }

        inventoryPanel.SetActive(isOpen);
    }
}
