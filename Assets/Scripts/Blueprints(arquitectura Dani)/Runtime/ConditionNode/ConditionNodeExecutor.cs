using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class ConditionNodeExecutor : BlueprintNodeExecutor<ConditionRuntimeNode>
    {
        protected override int OnExecute(ConditionRuntimeNode node, BlueprintDirector ctx,GameObject obj)
        {
            Debug.Log("Executing condition node");
            return 0;
        }
        
    }
}