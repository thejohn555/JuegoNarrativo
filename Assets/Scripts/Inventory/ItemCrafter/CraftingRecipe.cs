using UnityEngine;

[System.Serializable]
public class CraftingRecipe 
{
    public GameObject prefabA;
    public GameObject prefabB;
    public GameObject resultPrefab;

    public bool Matches(InventoryItem ingredientA, InventoryItem ingredientB)
    {
        return (Same(prefabA, ingredientA) && Same(prefabB, ingredientB) || Same(prefabA, ingredientB) && Same(prefabB, ingredientA));
    }
    
    // Se compara por guid (una instancia y su prefab son GameObjects distintos);
    // si no hay guid, por referencia al prefab.
    private static bool Same(GameObject recipePrefab, InventoryItem item)
    {
        if (recipePrefab == null || item.IsEmpty)
        {
            return false;
        }

        string recipeGuid = InventoryItem.GetGuid(recipePrefab);

        if (!string.IsNullOrEmpty(recipeGuid) && !string.IsNullOrEmpty(item.guid))
        {
            return recipeGuid == item.guid;
        }

        return recipePrefab == item.prefab;
    }
}
