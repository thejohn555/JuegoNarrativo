
using System.Collections.Generic;

public enum NodeTypes
{
    Trigger,
    Interactable,
    InteractableNpc,
    Reference
}

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public static class NodeGenTemplate
    {
        // tipo -> (prefijo de la clase generada, clase base, nombre del archivo,categoria para ordenalo visualmente)
        public static readonly Dictionary<NodeTypes, (string prefix, string baseClass, string file,string category)> Map = new()
        {
            { NodeTypes.Trigger,       ("OnTriggerEnter_", "OnTriggerEnterNode", "TriggerNodes.cs",  "Nodes/StartNodes/Triggers") },
            { NodeTypes.Interactable, ("OnInteract_",     "OnInteractNode",     "InteractNodes.cs", "Nodes/StartNodes/Interactable") },
            { NodeTypes.Reference, ("Ref_", "ReferenceNode", "ReferenceNodes.cs", "Nodes/References") },
            { NodeTypes.InteractableNpc,("OnInteract_Npc_","OnInteractNpcNode", "InteractNpcNodes.cs", "Nodes/StartNodes/InteractNpcNodes" ) },
        };
    }
}