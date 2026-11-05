using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Node(categoryPath:"Nodes/Inventory")]
    public class AddItemToInventoryNode:BlueprintNode
    {
        public const string RequiredObjectOption = "Object to Add";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {  
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GameObject>(RequiredObjectOption).Build();
        }
    }
}