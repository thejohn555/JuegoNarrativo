using Blueprints_arquitectura_Dani_.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Solo guarda los datos de los slots y expone la API del inventario.
// El dibujo está en InventoryUI y el crafteo en InventoryCrafter.
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; set; }
   
    [Header("Settings")]
    [SerializeField] public InventorySettings settings;
    public InventoryStorage storage { get; } = new InventoryStorage();
    private InventoryUI ui;
    public int HeldSlotIndex { get; set; } = -1;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ui = GetComponent<InventoryUI>();

        if (ui == null)
        {
            ui = gameObject.AddComponent<InventoryUI>();
        }
        
        storage.Changed += UpdateUI;
        
        // Si tenemos settings, dimensionamos el array lógico con las filas y columnas reales (ej. 4x4)
        if (settings != null)
        {
            storage.Resize(settings.rows * settings.columns);
        }
    }

    private void OnDestroy()
    {
        storage.Changed -= UpdateUI;
    }
    
    private void Start()
    {
        // Si el panel ya existe en la escena lo enlazamos; si no, lo construimos
        GameObject existingPanel = GameObject.Find(settings != null ? settings.inventoryName : "Inventory");

        if (existingPanel != null)
        {
            BindExistingPanel(existingPanel);
        }
        else if (settings != null)
        {
            InitializeInventory();
        }

        if (BlueprintsEventManager.Instance != null)
        {
            BlueprintsEventManager.Instance.AddItemToInventory = (prefab,guid) => storage.AddItem(prefab);
        }
    }

    public void InitializeInventory()
    {
        if (settings == null)
        {
            Debug.LogWarning("InventoryManager: InventorySettings are missing.");
            return;
        }

        storage.Resize(settings.rows * settings.columns);
        BindExistingPanel(InventoryBuilder.Build(settings));
    }

    public void BindExistingPanel(GameObject panel)
    {
        ui.Bind(panel);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (ui != null)
        {
            ui.Refresh(storage.Items, HeldSlotIndex);
        }
    }
}
