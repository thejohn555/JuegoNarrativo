using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    [Serializable]
    public class RuntimeNodeGroup
    {
        [SerializeReference] public List<RuntimeNode> Nodes = new();
    }
    [Serializable]
    public class BlueprintRuntimeGraph : ScriptableObject
    {
        public List<RuntimeNodeGroup> RuntimeNodes = new();
        public string sceneName;
    }
    
}