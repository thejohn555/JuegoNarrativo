
using System;
using Unity.GraphToolkit.Editor;

namespace DoorGraph.Editor.Node
{
    [Serializable]
    public abstract class DoorBaseNode:Unity.GraphToolkit.Editor.Node
    {
        const string OutputsOptionName = "Outputs";
        public abstract string Key { get; }
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>(OutputsOptionName).WithDefaultValue(1).Delayed().Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            int count = 1;
            GetNodeOptionByName(OutputsOptionName)?.TryGetValue(out count);
            for (int i = 0; i < count; i++)
            {
                context.AddOutputPort(i.ToString()).Build();
            }
        }
    }
}