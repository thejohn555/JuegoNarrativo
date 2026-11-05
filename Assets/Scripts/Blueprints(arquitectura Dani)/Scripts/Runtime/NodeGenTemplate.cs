
using System.Collections.Generic;

public enum NodeTypes
{
    Trigger,
    Interactable,
    InteractableNpc,
    ReferenceGo,
    ReferenceNpc,
    ReferenceInteractable,
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
            { NodeTypes.ReferenceGo, ("Ref_Go_", "ReferenceNode", "ReferenceGoNodes.cs", "Nodes/References/GameObject") },
            {NodeTypes.ReferenceInteractable,("Ref_Interactable_","ReferenceNode", "ReferenceInteractableNodes.cs",  "Nodes/References/Interactable")},
            {NodeTypes.ReferenceNpc,("Ref_Npc_", "ReferenceNode", "ReferenceNpcNodes.cs", "Nodes/References/Npc")},
            { NodeTypes.InteractableNpc,("OnInteract_Npc_","OnInteractNpcNode", "InteractNpcNodes.cs", "Nodes/StartNodes/InteractNpcNodes" ) },
        };
    }
}