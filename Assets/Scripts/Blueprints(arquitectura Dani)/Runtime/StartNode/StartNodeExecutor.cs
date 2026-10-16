using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class StartNodeExecutor : IBlueprintNodeExecutor<StartRuntimeNode>
    {
        public bool Execute(StartRuntimeNode node, BlueprintDirector ctx)
        {
            Debug.Log("Starting Blueprints");
            return true;
        }
    }
}