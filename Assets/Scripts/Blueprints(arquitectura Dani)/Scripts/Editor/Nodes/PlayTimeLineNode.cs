using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/TimeLine")]
    public class PlayTimeLineNode:BlueprintNode
    {
        public const string DirectorName = "TimeLine";
        public const string WaitTime= "WaitTime";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(DirectorName).Build();
            context.AddOption<bool>(WaitTime).Build();
        }
    }
}