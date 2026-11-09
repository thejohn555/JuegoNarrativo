using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.AddItemToInventory
{
    public class AddToiInventoryExecutor:BlueprintNodeExecutor<AddItemToInventoryRuntime>
    {
        protected override int OnExecute(AddItemToInventoryRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            
            if (string.IsNullOrEmpty(node.requiredGuid))
            {
                Debug.LogError("The node dont have a prefab assign");
                return -1;
            }
            else
            {
                Debug.Log($"Añadiendo: {node.itemName}, {node.itemIcon}, {node.requiredGuid}");
                BlueprintsEventManager.Instance.AddItemToInventory(node.itemName, node.itemIcon, node.requiredGuid);
                return 0;
            }
        }
    }
}