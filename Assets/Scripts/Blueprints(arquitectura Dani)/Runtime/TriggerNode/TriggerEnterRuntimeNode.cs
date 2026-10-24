using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.TriggerNode
{
    public class TriggerEnterRuntimeNode:RuntimeNode
    {
        public string trigger;

        public TriggerEnterRuntimeNode(string varName)
        {
            trigger = varName;
        }
    }
}