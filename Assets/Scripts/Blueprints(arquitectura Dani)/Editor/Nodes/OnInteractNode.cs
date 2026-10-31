using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor
{
    public abstract class OnInteractNode: BlueprintNode,IStartNode
    {
        public abstract string Key { get; }
        public const string RequiredObjectOption = "Interact required object";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {   
            context.AddOutputPort("Out").Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<GameObject>(RequiredObjectOption).Build();
        }
    }
}