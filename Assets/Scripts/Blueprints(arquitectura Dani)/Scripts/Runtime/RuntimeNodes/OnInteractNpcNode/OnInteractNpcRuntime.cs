namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.OnInteractNpcNode
{
    public class OnInteractNpcRuntime:RuntimeNode
    {
        public string interact;
        public string requiredGuid;
        public bool destroy;
        

        public OnInteractNpcRuntime(string varName, string requiredGuid,bool destroy)
        {
            interact = varName;
            this.requiredGuid = requiredGuid;
            this.destroy = destroy;
        }
    }
}