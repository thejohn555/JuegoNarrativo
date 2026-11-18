using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

// Solo craftea: trabaja sobre InventorySlots y no toca nada más del manager.
public class InventoryCrafter : MonoBehaviour
{
    public static InventoryCrafter Instance { get; private set; }
    
    [Header("Crafting Settings")]
    [SerializeField] public List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    // Si hay receta, craftea. Si no, mueve o intercambia como antes.
    public void TryCraftOrMove(int fromIndex, int toIndex)
    {
        if (InventoryManager.Instance == null || fromIndex == toIndex)
        {
            return;
        }

        InventoryStorage storage = InventoryManager.Instance.storage;

        // Si el slot de origen o el de destino están vacíos, solo se mueve
        if (!storage.TryGetItem(fromIndex, out InventoryItem itemA) || !storage.TryGetItem(toIndex, out InventoryItem itemB))
        {
            storage.MoveItem(fromIndex, toIndex);
            return;
        }

        if (TryGetRecipe(itemA, itemB, out CraftingRecipe recipe) && recipe.resultPrefab != null)
        {
            InventoryItem result = new InventoryItem(recipe.resultPrefab);

            if (string.IsNullOrEmpty(result.guid))
            {
                Debug.LogWarning($"La receta '{result.Name}' no tiene GUID: comprueba que el prefab resultado tiene BlueprintItemId");
            }

            storage.ClearSlot(fromIndex);
            storage.SetItem(toIndex, result);

            Debug.Log($"Successful crafting! Created: {result.Name}");
            
            if (BlueprintsEventManager.Instance != null && !string.IsNullOrEmpty(result.guid)) //<----Si no va quitar
            {
                BlueprintsEventManager.Instance.GetLastGuid(result.guid);
                BlueprintsEventManager.Instance.Raise(NodeTypes.OnItemColleted, gameObject);
            }
            else
            {
                Debug.LogError($"No hay blueprintEventManager");
            }
        }
        else
        {
            storage.MoveItem(fromIndex, toIndex);
        }
    }

    // Busca una receta que coincida con los dos ítems (sin importar el orden A+B o B+A)
    private bool TryGetRecipe(InventoryItem a, InventoryItem b, out CraftingRecipe recipe)
    {
        foreach (var ingredient in craftingRecipes)
        {
            if (ingredient.Matches(a, b))
            {
                recipe = ingredient;
                return true;
            }
        }

        recipe = default;
        return false;
    }
}

