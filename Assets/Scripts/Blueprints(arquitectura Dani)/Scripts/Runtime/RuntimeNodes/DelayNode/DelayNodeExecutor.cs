using System.Collections;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.DelayNode
{
    public class DelayNodeExecutor:BlueprintNodeExecutor<DelayNodeRuntime>
    {
        protected override int OnExecute(DelayNodeRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            ctx.PendingWait = node.delay;
            return 0;
        }
        
    }
}