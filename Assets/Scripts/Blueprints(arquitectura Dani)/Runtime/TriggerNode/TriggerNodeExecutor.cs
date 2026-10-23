using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.TriggerNode
{
    public class TriggerNodeExecutor:BlueprintNodeExecutor<TriggerEnterRuntimeNode>
    {
        protected override int OnExecute(TriggerEnterRuntimeNode node, BlueprintDirector ctx)
        {
            Debug.Log($"On Trigger Enter ");
            return 0; 
        }
    }
}