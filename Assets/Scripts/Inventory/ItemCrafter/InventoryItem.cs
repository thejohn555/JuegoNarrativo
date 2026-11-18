using Blueprints_arquitectura_Dani_.Runtime;
using Interactives;
using UnityEngine;

// Un ítem del inventario. Solo guarda el prefab; el icono y el guid salen de él.

public readonly struct InventoryItem 
{
    public readonly GameObject prefab;
    public readonly Sprite icon; // sprite del SpriteRenderer del prefab (cacheado)
    public readonly string guid; // BlueprintItemId del prefab

    public InventoryItem(GameObject _prefab)
    {
        prefab = _prefab;
        icon = GetIcon(_prefab);
        guid = GetGuid(_prefab);
    }

    public bool IsEmpty => prefab == null;

    // Nombre visible: el del propio prefab
    public string Name => prefab != null ? prefab.name.Replace("(Clone)", "").Trim() : "";
    public static string GetGuid(GameObject prefab) => prefab != null && prefab.TryGetComponent<BlueprintItemId>(out var id) ? id.Guid : null;

    public static Sprite GetIcon(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        // Icono personalizado para el inventario (tiene prioridad)
        if (prefab.TryGetComponent<CollectableObject>(out var collectable) && collectable.inventoryIcon != null)
        {
            return collectable.inventoryIcon;
        }

        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        return spriteRenderer != null ? spriteRenderer.sprite : null;
    }
}
