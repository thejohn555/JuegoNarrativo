using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.OnItemCollected
{
    public class OnItemCollectedRuntime:RuntimeNode
    {
        public string requiredGuid;
        public OnItemCollectedRuntime(string requiredGuid)
        {
            this.requiredGuid = requiredGuid;
        }
    }
}