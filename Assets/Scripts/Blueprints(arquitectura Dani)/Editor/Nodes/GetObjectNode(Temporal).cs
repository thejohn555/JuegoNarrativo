using Blueprints_arquitectura_Dani_.Editor.Nodes;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    public class GetObjectNode_Temporal:BlueprintNode
    {
        public const string ObjectPort = "Object";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();                       // ejecución
            context.AddInputPort<string>(ObjectPort).Build();         // el cable del nodo objeto
            context.AddOutputPort("Out").Build();
        }
    }
}