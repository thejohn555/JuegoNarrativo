using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Node(categoryPath:"Nodes/StartNodes")]
    public class OnUseItem:BlueprintNode,IStartNode
    {
        public const string ObjectPort = "OnItemUsed";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        { 
            context.AddInputPort<string>(ObjectPort).Build();
            context.AddOutputPort("Out").Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GameObject>("ItemUsed").Build();
        }
    }
}