using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Interactives
{
    public class CollectableObject : Interactive
    {
        [Header("Item")]
        [SerializeField] private GameObject itemPrefab; // el prefab asset de este ítem (arrastrar en el Inspector)

        [Header("Inventory Icon (Opcional)")]
        [Tooltip("Si se asigna, el inventario usa este sprite en vez del sprite del objeto en la escena")]
        public Sprite inventoryIcon;
        
        [Header("Crafting Ingredients (Opcional)")]
        public GameObject ingredientA;
        public GameObject ingredientB;

        [HideInInspector] public Sprite itemIcon;
        
        public override void Interact(GameObject interactor)
        {
            Debug.Log("Se ha interactuado con el objeto");
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("No se encontró ningún InventoryManager activo en la escena.");
                return;
            }

            if (itemPrefab == null)
            {
                Debug.LogError($"'{name}' no tiene itemPrefab asignado: no se puede añadir al inventario.", this);
                return;
            }

            itemIcon = InventoryItem.GetIcon(itemPrefab);

            string guid = InventoryItem.GetGuid(itemPrefab);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning($"'{itemPrefab.name}' no tiene BlueprintItemId, no funcionará en los blueprints", this);
            }

            bool added = InventoryManager.Instance.storage.AddItem(itemPrefab);

            if (added)
            {
                Debug.Log("Añadido");
                // Si se añadió correctamente al inventario, desaparece de la escena
                BlueprintsEventManager.Instance.GetLastGuid(guid);
                BlueprintsEventManager.Instance.Raise(NodeTypes.OnItemColleted, this.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("El inventario está lleno.");
            }
        }
    }
}