using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.ChangeNpcState
{
    public class ChangeNpcStateRuntime:RuntimeNode
    {
        public string npcState;
        public string npcId;

        public ChangeNpcStateRuntime(string npcState, string npcId)
        {
            this.npcState = npcState;
            this.npcId = npcId;
        }
    }
}