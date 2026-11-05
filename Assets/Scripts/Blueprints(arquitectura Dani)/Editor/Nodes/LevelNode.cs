using UnityEditor.Graphs;
using Unity.GraphToolkit.Editor;
using Node = Unity.GraphToolkit.Editor.Node;
namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes
{
    [Node(categoryPath:"Nodes/SetUp")]
    public class LevelNode:Node
    {
        protected override void OnDefineOptions(Node.IOptionDefinitionContext context)
        {
            context.AddOption<string>("GraphLevel").Build();
            
        }
    }
}