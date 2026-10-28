
using System.Collections.Generic;

public enum NodeTypes
{
    Trigger,
    Interactuable
}

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public static class NodeGenTemplate
    {
        // tipo -> (prefijo de la clase generada, clase base, nombre del archivo)
        public static readonly Dictionary<NodeTypes, (string prefix, string baseClass, string file)> Map = new()
        {
            { NodeTypes.Trigger,       ("OnTriggerEnter_", "OnTriggerEnterNode", "TriggerNodes.cs") },
           
        };
    }
}