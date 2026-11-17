using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.OnDialogueEnd
{
    public class OnDialogueEndRuntime:RuntimeNode
    {
        public string goName;
        public OnDialogueEndRuntime(string goName)
        {
            this.goName = goName;
        }
        
    }
}