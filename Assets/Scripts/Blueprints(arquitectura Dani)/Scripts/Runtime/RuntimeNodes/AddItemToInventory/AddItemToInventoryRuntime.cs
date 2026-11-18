using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.AddItemToInventory
{
    public class AddItemToInventoryRuntime : RuntimeNode
    {
        public string requiredGuid;
        public GameObject itemPrefab;
        public AddItemToInventoryRuntime(string requiredGuid, GameObject itemPrefab)
        {
            this.requiredGuid = requiredGuid;
            this.itemPrefab = itemPrefab;
        }
    }
}