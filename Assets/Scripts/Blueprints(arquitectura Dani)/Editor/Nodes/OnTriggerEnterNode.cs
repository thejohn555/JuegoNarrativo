using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor
{
    public abstract class OnTriggerEnterNode : BlueprintNode,IStartNode
    {
        public const string targetPort= "Target";
        public abstract string TriggerKey { get; }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {   
            context.AddOutputPort("Out").Build();
        }
    }
}
