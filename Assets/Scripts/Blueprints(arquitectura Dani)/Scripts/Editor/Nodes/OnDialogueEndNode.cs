using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    public class OnDialogueEndNode:BlueprintNode,IStartNode
    {
        public const string ObjectPort= "Object";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort("Out").Build();
            context.AddInputPort<string>(ObjectPort);
        }
    }
}