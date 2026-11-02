using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.InteractNode
{
    public class InteractRuntimeNode : RuntimeNode
    {
        public string interact;
        public string requiredGuid;
        

        public InteractRuntimeNode(string varName, string requiredGuid)
        {
            interact = varName;
            this.requiredGuid = requiredGuid;
        }
    }
}