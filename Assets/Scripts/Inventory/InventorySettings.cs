using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName = "InventorySettings", menuName = "Assets/Scripts/Inventory/InventorySettings")]
public class InventorySettings : ScriptableObject
{
    //Setings del inventario. La ventana edita los parametros y el builder los lee
    [Header("Inventary Name")]
    [SerializeField] public string inventoryName = "Inventario1"; //Si se crea uno con el mismo nombre se elimina el segundo
    
    [Header("Dimensions")] //Tamaño del inventario
    [Min(1)] public int rows = 1;
    [Min(1)] public int columns = 1;

    [Header("Slots")] //Casillas del inventario
    [SerializeField] public Vector2 slotSize = new Vector2(80, 80);
    [SerializeField] public Vector2 spacing = new Vector2(8, 8);
    [Min(0)] public int padding = 12; //Distancia entre las casillas y el borde del inventario
    [SerializeField] public Sprite slotSprite;
    [SerializeField] public Color slotColor;

    [Header("Panel")] //Fondo del inventario
    [SerializeField] public Sprite panelSprite;
    [SerializeField] public Color panelColor;
    [Tooltip("If a panel with that name already exists, it replaces it.")]
    public bool replaceExisting = true;
    
    public int SlotCount => rows * columns;
    
    public Vector2 panelSize => new Vector2 (
        padding * 2 + columns * slotSize.x + (columns - 1) * spacing.x,
        padding * 2 + rows * slotSize.y + (rows - 1) * spacing.y);
    
    
    void OnValidate() //Evita que se pongan valores erroneos. Sirve para poner default
    {
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
        slotSize = Vector2.Max(slotSize, Vector2.one * 8f);
        spacing = Vector2.Max(spacing, Vector2.zero);
        
        if (string.IsNullOrWhiteSpace(inventoryName))
        {
            inventoryName = "InventoryPanel";
        }
    }
}
