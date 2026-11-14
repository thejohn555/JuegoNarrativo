using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.OnItemCollected
{
    public class OnitemCollectedExecutor:BlueprintNodeExecutor<OnItemCollectedRuntime>
    {
    protected override int OnExecute(OnItemCollectedRuntime node, BlueprintDirector ctx, GameObject obj)
    {
        Debug.Log("Executing OnItemCollected");
        if (string.IsNullOrEmpty(node.requiredGuid))
        {
            return 0;
        }
        string last = BlueprintMediator.Instance.LastGuidPick;
        if (last == node.requiredGuid)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
    }
}