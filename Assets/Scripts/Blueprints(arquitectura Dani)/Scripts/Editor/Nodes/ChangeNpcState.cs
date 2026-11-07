using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Unity.GraphToolkit.Editor;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes
{
    [Node(categoryPath:"Nodes/General/Dialogue")]
    public class ChangeNpcState:BlueprintNode
    {
        public const string NpcId = "Npc_Id";
        public const string NpcStateName = "NpcState";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("In").Build();
            context.AddOutputPort("out").Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(NpcId).Build();
            context.AddOption<string>(NpcStateName).Build();
        }
    }
}