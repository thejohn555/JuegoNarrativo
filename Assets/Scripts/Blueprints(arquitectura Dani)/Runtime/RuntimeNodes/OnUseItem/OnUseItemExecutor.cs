using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.OnUseItem
{
    public class OnUseItemExecutor:BlueprintNodeExecutor<OnUseItemRuntime>
    {
        protected override int OnExecute(OnUseItemRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            Debug.Log("Executing OnUseItem");
            return 1;
        }
    }
}