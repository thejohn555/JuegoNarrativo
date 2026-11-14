using Blueprints_arquitectura_Dani_.Editor.Nodes;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    public class StartBark:BlueprintNode
    {
        public const string ObjectPort= "NPC";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
            context.AddInputPort<string>(ObjectPort).Build();
        }
    }
}