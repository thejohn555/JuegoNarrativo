using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.GetObjectNode_Temporal_
{
    public class GetObjectExecutor:BlueprintNodeExecutor<GetObjectNodeRuntime>
    {
        
        protected override int OnExecute(GetObjectNodeRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            var target = BlueprintDictionaryManager.Instance.Resolve(NodeTypes.Reference, node.goName);
            if (target == null)
            {
                Debug.LogWarning($"No existe el objeto '{node.goName}' en la escena");
                return -1;
            }
            target.SetActive(false);
            return 0;

        }
    }
}