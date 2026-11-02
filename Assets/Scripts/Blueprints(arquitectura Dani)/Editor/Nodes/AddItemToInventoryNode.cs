using Blueprints_arquitectura_Dani_.Editor.Nodes;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
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