using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/Objects")]
    public class MoveToNode:BlueprintNode
    {
        public const string PositionOption = "Position Option";
        public const string TimeOption = "Time Option";
        public const string ObjectPort= "Object";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();
            context.AddInputPort<string>(ObjectPort);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
          context.AddOption<Vector2>(PositionOption).Build();
          context.AddOption<float>(TimeOption).Build();
          
        }
    }
}