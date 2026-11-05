using Blueprints_arquitectura_Dani_.Editor.Nodes;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    public abstract class ReferenceNode:BlueprintNode
    {
        public abstract string Key { get; }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<string>("Object").Build();   
        }
    }
}