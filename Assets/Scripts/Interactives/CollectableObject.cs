using Blueprints_arquitectura_Dani_.Runtime;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Interactives
{
    public class CollectableObject : Interactive
    {
        [Header("Crafting Ingredients (Opcional)")]
        public GameObject ingredientA;
        public GameObject ingredientB;
        
        public override void Interact(GameObject interactor)
        {
            Debug.Log("Se ha interactuado con el objeto");
            // Extrae el sprite automáticamente del objeto visual en la escena
            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Sprite itemSprite = spriteRenderer != null ? spriteRenderer.sprite : null;
            string itemName = gameObject.name;

            if (InventoryManager.Instance != null)
            {
                Debug.Log("Añadido");
                string guid = GetComponent<BlueprintItemId>()?.Guid;//Nuevo
                if (string.IsNullOrEmpty(guid))//Nuevo
                    Debug.LogWarning($"'{name}' no tiene BlueprintItemId, no funcionará en los blueprints", this);//Nuevo
                bool added = InventoryManager.Instance.AddItem(itemName, itemSprite,guid);
                
                if (added)
                {
                    // Si se añadió correctamente al inventario, desaparece de la escena
                    Destroy(gameObject);
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.collectObjectSound, 0.7f);
                }
                else
                {
                    Debug.Log("El inventario está lleno.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró ningún InventoryManager activo en la escena.");
            }
        }
    }
}