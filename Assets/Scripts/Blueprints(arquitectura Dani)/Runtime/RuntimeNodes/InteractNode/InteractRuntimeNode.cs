using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.InteractNode
{
    public class InteractRuntimeNode : RuntimeNode
    {
        public string interact;
        public GameObject gameObject;

        public InteractRuntimeNode(string varName, GameObject required)
        {
            interact = varName;
            gameObject = required;
        }
    }
}