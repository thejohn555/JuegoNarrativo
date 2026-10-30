using Blueprints_arquitectura_Dani_.Editor.Nodes;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    public abstract class GeneratedBaseClass: BlueprintNode
    {
        public abstract string Key { get; }
        public abstract string Level { get; }
        
        protected virtual void OnDefinePorts(IPortDefinitionContext context)
        {   
            context.AddOutputPort("Out").Build();
        }

    }
}