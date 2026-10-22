using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor
{
    public class OnTriggerEnterNode : BlueprintNode,IStartNode
    {
        public const string targetPort= "Target";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<GameObject>(targetPort).Build();
            context.AddOutputPort("Out").Build();
        }
    }
}
