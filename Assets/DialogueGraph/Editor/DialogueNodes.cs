using System;
using Unity.GraphToolkit.Editor;

namespace DialogueGraph.Editor
{
    [Serializable]
    public class StartNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort("Out").Build();
        }
    }
    
    [Serializable]
    public class EndNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
        }
    }
    
    [Serializable]
    public class DialogueNode : Node
    {
        public const string PortDialogue = "Dialogue Text";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("Out").Build();

            context.AddInputPort<string>(PortDialogue).Build();
        }
    }
}