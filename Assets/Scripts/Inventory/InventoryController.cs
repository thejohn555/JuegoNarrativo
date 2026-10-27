using Assets.Scripts.EventBuses;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Configuración")]
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
    }

    public void ToggleInventory(OnScrollWheelPressed eventArgs)
    {
        // Alternamos el estado
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }
}
