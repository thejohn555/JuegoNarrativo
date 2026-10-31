namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.OnUseItem
{
    public class OnUseItemRuntime: RuntimeNode
    {
        public string itemName;
        public string targetName;
        
        public OnUseItemRuntime(string itemName, string targetName)
        {
            this.itemName = itemName;
            this.targetName = targetName;
        }
    }
}