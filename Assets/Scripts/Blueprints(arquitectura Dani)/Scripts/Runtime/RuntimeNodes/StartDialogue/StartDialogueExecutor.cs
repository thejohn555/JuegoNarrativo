using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.StartDialogue
{
    public class StartDialogueExecutor:BlueprintNodeExecutor<StartDialogueRuntime>
    {
        protected override int OnExecute(StartDialogueRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            if(string.IsNullOrEmpty(node.goName)) return -1;
            return 0;
        }
    }
}