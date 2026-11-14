using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.ChangeNpcState
{
    public class ChangeNpcStateExecutor:BlueprintNodeExecutor<ChangeNpcStateRuntime>
    {
        protected override int OnExecute(ChangeNpcStateRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            if (string.IsNullOrEmpty(node.npcState) || string.IsNullOrEmpty(node.npcId)) return-1;
            Debug.Log($"Cmabiado id={node.npcId}, state={node.npcState}");
            BlueprintsEventManager.Instance.ChangeNpcState(node.npcId, node.npcState);
            return 0;
        }
    }
}