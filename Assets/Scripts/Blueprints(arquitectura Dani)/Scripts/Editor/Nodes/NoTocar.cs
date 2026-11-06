using System;
using Unity.GraphToolkit.Editor;

[Serializable]
[Node(categoryPath:"Nodes/General/SetUp")]
public class NoTocar : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<bool>("condition1").WithDisplayName("Variable").Build();
        context.AddInputPort<int>("condition2").WithDisplayName("Variable").Build();
        context.AddInputPort<float>("condition3").WithDisplayName("Variable").Build();
        context.AddInputPort<double>("condition4").WithDisplayName("Variable").Build();
        

    }
}