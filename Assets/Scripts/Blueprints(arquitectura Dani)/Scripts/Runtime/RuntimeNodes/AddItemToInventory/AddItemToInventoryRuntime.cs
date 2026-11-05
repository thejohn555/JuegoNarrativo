using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.AddItemToInventory
{
    public class AddItemToInventoryRuntime : RuntimeNode
    {
        public string requiredGuid;
        public string itemName;
        public Sprite itemIcon;
        public AddItemToInventoryRuntime(string requiredGuid,string itemName, Sprite itemIcon)
        {
            this.requiredGuid = requiredGuid;
            this.itemName = itemName;
            this.itemIcon = itemIcon;
        }
    }
}