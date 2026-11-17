using System;
using UnityEngine;

// Los datos de los ítems del inventario y las operaciones sobre ellos (leer, mover, colocar, vaciar).
// No conoce la UI: avisa con el evento Changed y el InventoryManager repinta.
public class InventoryStorage : MonoBehaviour
{
    public InventoryItem[] items = new InventoryItem[0];
    public event Action Changed;
    public int Count => items.Length;
    public InventoryItem[] Items => items; //Lectura de la UI
    
    // Recrealos huecos vacíos
    public void Resize(int size)
    {
        items = new InventoryItem[size];
    }
    
    // Añade un ítem en el primer slot libre. Recibe el prefab del ítem.
    public bool AddItem(GameObject prefab)
    {
        if (prefab == null) return false;

        if (items.Length == 0)
        {
            Debug.LogError("InventoryStorage: no hay slots. ¿Falta InventorySettings en el InventoryManager?");
            return false;
        }

        int freeIndex = FindFirstEmpty();
        if (freeIndex < 0)
        {
            Debug.LogWarning("The inventory is full!");
            return false;
        }

        SetItem(freeIndex, new InventoryItem(prefab));
        return true;
    }
    
    // Devuelve el ítem de un slot (false si está vacío o el índice  es inválido)
    public bool TryGetItem(int index, out InventoryItem item)
    {
        item = default;

        if (!IsValid(index) || items[index].IsEmpty)
        {
            return false;
        }

        item = items[index];
        return true;
    }
    
    public void MoveItem(int fromIndex, int toIndex)
    {
        if (!IsValid(fromIndex) || !IsValid(toIndex))
        {
            return;
        }

        Debug.Log($"[Move] {fromIndex} -> {toIndex}");
        (items[fromIndex], items[toIndex]) = (items[toIndex], items[fromIndex]);
        Changed?.Invoke();
    }
    
    // Coloca un ítem en un slot
    public void SetItem(int index, InventoryItem item)
    {
        if (!IsValid(index))
        {
            return;
        }

        items[index] = item;
        Changed?.Invoke();
    }
    
    // Vacía un slot (por ejemplo, al gastar un objeto)
    public void ClearSlot(int index)
    {
        if (!IsValid(index)) return;

        items[index] = default;
        Changed?.Invoke();
    }
    
    // Índice del primer slot libre, o -1 si está lleno
    public int FindFirstEmpty()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].IsEmpty) return i;
        }
        return -1;
    }
    
    private bool IsValid(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            return true;
        }
    
        return false;
    }
}
