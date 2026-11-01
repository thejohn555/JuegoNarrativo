using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.SetActive
{
    public class SetActiveExecutor:BlueprintNodeExecutor<SetActiveRuntimeNode>
    {
        protected override int OnExecute(SetActiveRuntimeNode node, BlueprintDirector ctx, GameObject obj)
        {
            if (string.IsNullOrEmpty(node.goName))
            {
                obj.SetActive(node.active);
                return 0;
            }
            var target = BlueprintDictionaryManager.Instance.Resolve(NodeTypes.Reference, node.goName);
            target.SetActive(node.active);
            return 0;
        }
    }
}