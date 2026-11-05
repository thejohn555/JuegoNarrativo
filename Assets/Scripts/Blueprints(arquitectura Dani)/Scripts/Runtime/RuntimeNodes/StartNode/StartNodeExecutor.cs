using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class StartNodeExecutor : BlueprintNodeExecutor<StartRuntimeNode>
    {
        protected override int OnExecute(StartRuntimeNode node, BlueprintDirector ctx,GameObject obj)
        {
            Debug.Log("Starting Blueprints");
            return 0;
        }
    }
}