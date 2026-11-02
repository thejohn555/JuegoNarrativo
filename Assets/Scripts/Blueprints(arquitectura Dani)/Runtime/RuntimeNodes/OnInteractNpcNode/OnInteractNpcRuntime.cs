namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.OnInteractNpcNode
{
    public class OnInteractNpcRuntime:RuntimeNode
    {
        public string interact;
        public string requiredGuid;
        

        public OnInteractNpcRuntime(string varName, string requiredGuid)
        {
            interact = varName;
            this.requiredGuid = requiredGuid;
        }
    }
}