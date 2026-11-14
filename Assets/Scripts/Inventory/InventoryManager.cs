using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; set; }
   
    [Header("Settings")]
    [SerializeField] public InventorySettings settings;
    
    [System.Serializable]
    public struct CraftingRecipe
    {
        public string ingredientA;
        public string ingredientB;
        public string resultName;
        public Sprite resultIcon;
        public GameObject resultPrefab;
        [HideInInspector] public string resultGuid; //Nuevo
    }

    [Header("Crafting Settings")]
    [SerializeField] private List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();

    private struct InventoryItem
    {
       public string itemName;
       public Sprite icon;
       public string guid; //Nuevo
    }

    [SerializeField] private InventoryItem[] inventorySlots;
    private Transform panelTransform;
    private List<Image> slotIconImages = new List<Image>();
    private List<TextMeshProUGUI> slotIconNames = new List<TextMeshProUGUI>();

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

        // Si tenemos settings, dimensionamos el array lógico con las filas y columnas reales (ej. 4x4)
        if (settings != null)
        {
            inventorySlots = new InventoryItem[settings.rows * settings.columns];
        }
    }

    private void Start()
    {
        // Al arrancar, en vez de crear uno nuevo a la fuerza, buscamos si ya existe el panel en la escena
        GameObject existingPanel = GameObject.Find(settings != null ? settings.inventoryName : "Inventory");

        if (existingPanel != null)
        {
            // Si ya existe (lo creaste con el editor), enlazamos sus referencias visuales existentes
            BindExistingPanel(existingPanel);
        }
        else if (settings != null)
        {
            // Si no existe, lo construimos por primera vez
            InitializeInventory();
        }
        if (BlueprintsEventManager.Instance != null)
            BlueprintsEventManager.Instance.AddItemToInventory = AddItem;
    }

    public void InitializeInventory()
    {
        if (settings == null)
        {
            Debug.LogWarning("InventoryManager: InventorySettings are missing.");
            return;
        }

        inventorySlots = new InventoryItem[settings.rows * settings.columns];

#if UNITY_EDITOR
        // Solo en el editor: construir el panel si no existe
        GameObject panel = InventoryBuilder.Build(settings);
        BindExistingPanel(panel);
#else
    Debug.LogError("InventoryManager: the inventory panel was not found in the scene. Build it in the editor before making a build.");
#endif
    }
   
    public bool AddItem(string itemName, Sprite icon,string guid = null) //Nuevo
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            if (settings != null)
            {
                inventorySlots = new InventoryItem[settings.rows * settings.columns];
            }
            else
            {
                Debug.LogError("InventoryManager has no InventorySettings assigned.");
                return false;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].icon == null)
            {
                inventorySlots[i] = new InventoryItem { itemName = itemName, icon = icon, guid = guid };
                UpdateUI();
                return true;
            }
        }
        
        Debug.LogWarning("The inventory is full!");
        return false;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slotIconImages.Count; i++)
        {
            
            bool hasItem = i < inventorySlots.Length && inventorySlots[i].icon != null && i != HeldSlotIndex;
            
            if (hasItem)
            {
                slotIconImages[i].sprite = inventorySlots[i].icon;
                slotIconImages[i].enabled = true;
            }
            else
            {
                slotIconImages[i].sprite = null;
                slotIconImages[i].enabled = false;
            }
            
            if (i < slotIconNames.Count && slotIconNames[i] != null)
            {
                slotIconNames[i].text = hasItem ? inventorySlots[i].itemName : ""; 
            }
        }
    }
    
    // Devuelve los datos de un slot (false si está vacío o el índice no es válido)
    public bool TryGetItem(int index, out string itemName, out Sprite icon)
    {
        itemName = null;
        icon = null;

        if (inventorySlots == null || index < 0 || index >= inventorySlots.Length || inventorySlots[index].icon == null)
        {
            return false;
        }

        itemName = inventorySlots[index].itemName;
        icon = inventorySlots[index].icon;
        Debug.Log($"[TryGetItem] slot {index} | guid: '{inventorySlots[index].guid}'");
        return true;
    }
    public bool TryGetItem(int index, out string itemName, out Sprite icon, out string guid)//Nuevo
    {
        guid = null;
        if (!TryGetItem(index, out itemName, out icon)) return false;
        guid = inventorySlots[index].guid;
        return true;
    }

    // Vacía un slot (por ejemplo, al gastar un objeto)
    public void ClearSlot(int index)
    {
        if (inventorySlots == null || index < 0 || index >= inventorySlots.Length)
        {
            return;
        }

        inventorySlots[index] = new InventoryItem();
        UpdateUI();
    }
    
    // Mueve o intercambia un objeto de un slot a otro
    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= inventorySlots.Length || toIndex < 0 || toIndex >= inventorySlots.Length)
        {
            return;
        }

        // Intercambiamos los datos en el array lógico
        (inventorySlots[fromIndex], inventorySlots[toIndex]) = (inventorySlots[toIndex], inventorySlots[fromIndex]);
        
        UpdateUI();
    }
    
    // Comprueba si se puede craftear o si debe hacer un intercambio normal
    public void TryCraftOrMove(int fromIndex, int toIndex)
    {
        
        if (fromIndex < 0 || fromIndex >= inventorySlots.Length || toIndex < 0 || toIndex >= inventorySlots.Length)
        {
            return;
        }

        string itemA = inventorySlots[fromIndex].itemName;
        string itemB = inventorySlots[toIndex].itemName;

        // Comprobamos si el slot de destino está vacío (si está vacío, solo movemos)
        if (string.IsNullOrEmpty(itemB))
        {
            MoveItem(fromIndex, toIndex);
            return;
        }

        // Buscamos si hay una receta válida para estos dos ítems
        CraftingRecipe matchedRecipe = GetValidRecipe(itemA, itemB);

        if (matchedRecipe.resultIcon != null)
        {
            string resultGuid = null;
            if (matchedRecipe.resultPrefab != null &&
                matchedRecipe.resultPrefab.TryGetComponent<BlueprintItemId>(out var itemId))
            {
                resultGuid = itemId.Guid;
            }

            if (string.IsNullOrEmpty(resultGuid))
            {
                Debug.LogWarning($"La receta '{matchedRecipe.resultName}' no tiene GUID: comprueba que resultPrefab tiene BlueprintItemId");
            }
            Debug.Log($"[Craft] '{matchedRecipe.resultName}' | prefab: {(matchedRecipe.resultPrefab == null ? "NULL" : matchedRecipe.resultPrefab.name)} | guid: '{resultGuid}'");
            inventorySlots[fromIndex] = new InventoryItem { itemName = null, icon = null };
            inventorySlots[toIndex] = new InventoryItem
            {
                itemName = matchedRecipe.resultName, 
                icon = matchedRecipe.resultIcon,
                guid = resultGuid,//Nuevo
            };
            BlueprintsEventManager.Instance.GetLastGuid(resultGuid);
            Debug.Log($"Successful crafting! Created:{matchedRecipe.resultName}");
            
            BlueprintsEventManager.Instance.Raise(NodeTypes.OnItemColleted,this.gameObject);
            UpdateUI();
        }
        else
        {
            // Si no hay receta, se comportan como antes: intercambian posición
            MoveItem(fromIndex, toIndex);
        }
    }

    // Busca si existe una receta que coincida con los dos ítems (sin importar el orden A+B o B+A)
    private CraftingRecipe GetValidRecipe(string itemA, string itemB)
    {
        foreach (var recipe in craftingRecipes)
        {
            bool matchDirect = (recipe.ingredientA == itemA && recipe.ingredientB == itemB);
            bool matchReverse = (recipe.ingredientA == itemB && recipe.ingredientB == itemA);

            if (matchDirect || matchReverse)
            {
                return recipe;
            }
        }
        return new CraftingRecipe(); // Devuelve una receta vacía si no encuentra coincidencia
    }
    
    // Vincula las referencias del panel que ya está dibujado en la escena
    public void BindExistingPanel(GameObject panel)
    {
        panelTransform = panel.transform;
        slotIconImages.Clear();
        slotIconNames.Clear();

        foreach (Transform slotChild in panelTransform)
        {
            Transform iconTransform = slotChild.Find("Icon");
            if (iconTransform != null)
            {
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

        UpdateUI();
    }
}
