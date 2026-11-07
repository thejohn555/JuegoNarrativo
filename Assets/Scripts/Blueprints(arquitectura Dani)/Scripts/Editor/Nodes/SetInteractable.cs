using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/Object")]
    public class SetInteractable:BlueprintNode
    {
        public const string BoolOption = "Set Interactable";
        public const string ObjectPort = "Prefab";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
            context.AddInputPort<string>(ObjectPort);
        }
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<bool>(BoolOption).Build();
        }
        
    }
}