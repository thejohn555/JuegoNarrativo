using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/Start")]
    public class OnItemCollected:BlueprintNode,IStartNode
        
    {
        public const string RequiredObjectOption = "Object type";
        
        protected override void OnDefinePorts(Node.IPortDefinitionContext context)
        {   
            context.AddOutputPort("Out").Build();
        }

        protected override void OnDefineOptions(Node.IOptionDefinitionContext context)
        {
            context.AddOption<GameObject>(RequiredObjectOption).Build();
        }
    }
}