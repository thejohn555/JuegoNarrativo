using Blueprints_arquitectura_Dani_.Editor.Nodes;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    public class SetActiveNode: BlueprintNode
    {
        public const string BoolOption = "Set Active";
        public const string ObjectPort= "Object";
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