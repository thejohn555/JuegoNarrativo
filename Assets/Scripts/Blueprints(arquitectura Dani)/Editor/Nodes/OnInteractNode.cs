using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Editor
{
    public abstract class OnInteractNode: BlueprintNode,IStartNode
    {
        public abstract string Key { get; }
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {   
            context.AddOutputPort("Out").Build();
        }
        
    }
}