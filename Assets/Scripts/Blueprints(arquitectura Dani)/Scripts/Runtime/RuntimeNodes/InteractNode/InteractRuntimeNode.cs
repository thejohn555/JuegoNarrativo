using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.InteractNode
{
    public class InteractRuntimeNode : RuntimeNode
    {
        public string interact;
        public string requiredGuid;
        public bool destroy;
        

        public InteractRuntimeNode(string varName, string requiredGuid,bool destroy)
        {
            interact = varName;
            this.requiredGuid = requiredGuid;
            this.destroy = destroy;
        }
    }
}