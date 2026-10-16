using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class ConditionNodeExecutor : IBlueprintNodeExecutor<ConditionRuntimeNode>
    {
        public bool Execute(ConditionRuntimeNode node, BlueprintDirector ctx)
        {
            Debug.Log("Executing condition node");
            return true;
        }
    }
}