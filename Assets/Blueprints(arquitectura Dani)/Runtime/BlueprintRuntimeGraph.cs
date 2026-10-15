using System.Collections.Generic;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    [CreateAssetMenu(fileName = "BlueprintRuntimeGraph", menuName = "Scriptable Objects/BlueprintRuntimeGraph")]
    public class BlueprintRuntimeGraph:ScriptableObject
    {
            [SerializeReference]
            public List<RuntimeNode> RuntimeNodes = new();
    }
}