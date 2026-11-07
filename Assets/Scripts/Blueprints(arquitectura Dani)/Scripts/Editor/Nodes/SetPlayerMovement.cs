using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/Player")]
    public class SetPlayerMovement:BlueprintNode
    {
        public const string BoolOption = "Set";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
        }
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<bool>(BoolOption).Build();
        }
        
    }
}