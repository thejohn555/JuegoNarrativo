using System;
using Unity.GraphToolkit.Editor;

namespace DialogueGraph.Editor
{
    public static class PortNames
    {
        public const string PortDialogue = "Dialogue Text";
        public const string PortSpeaker = "Speaker";
        public const string PortOutput = "Out";
        public const string PortInput = "In";
        public const string PortChoiceOutput = "Choice ";
        public const string PortChoiceText = "Choice Text ";
    }

    [Serializable]
    public class StartNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(PortNames.PortOutput).Build();
        }
    }
    
    [Serializable]
    public class EndNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(PortNames.PortInput).Build();
        }
    }
    
    [Serializable]
    public class DialogueNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(PortNames.PortInput).Build();
            context.AddOutputPort(PortNames.PortOutput).Build();

            context.AddInputPort<string>(PortNames.PortSpeaker).Build();
            context.AddInputPort<string>(PortNames.PortDialogue).Build();
            context.AddInputPort<float>("Text Speed").Build();
        }
    }

    [Serializable]
    public class ChoiceNode : Node
    {
        public const string optionID = "portCount";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(PortNames.PortInput).Build();

            context.AddInputPort<string>(PortNames.PortSpeaker).Build();
            context.AddInputPort<string>(PortNames.PortDialogue).Build();
            context.AddInputPort<float>("Text Speed").Build();

            var option = GetNodeOptionByName(optionID);
            option.TryGetValue(out int portCount);
            for (int i = 0; i < portCount; i++)
            {
                context.AddInputPort<string>($"{PortNames.PortChoiceText} {i}").Build();
                context.AddOutputPort($"{PortNames.PortChoiceOutput} {i}").Build();
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>(optionID).WithDefaultValue(2).Delayed();
        }
    }
}