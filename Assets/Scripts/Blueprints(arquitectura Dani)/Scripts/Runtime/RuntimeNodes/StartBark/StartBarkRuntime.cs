using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.StartBark
{
    public class StartBarkRuntime:RuntimeNode
    {
        public string NpcName;
        
        public StartBarkRuntime(string npcName)
        {
            this.NpcName = npcName;
        }
    }
}