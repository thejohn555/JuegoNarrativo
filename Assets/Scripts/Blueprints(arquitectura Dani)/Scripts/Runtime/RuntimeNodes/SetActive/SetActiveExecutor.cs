using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.SetActive
{
    public class SetActiveExecutor:BlueprintNodeExecutor<SetActiveRuntimeNode>
    {
        protected override int OnExecute(SetActiveRuntimeNode node, BlueprintDirector ctx, GameObject obj)
        {
            var mgr = BlueprintDictionaryManager.Instance;
            Debug.Log($"[SetActive] key='{node.goName}' | Go:{mgr.GetObjects(NodeTypes.ReferenceGo).Count} " +
                      $"Interactable:{mgr.GetObjects(NodeTypes.ReferenceInteractable).Count} " +
                      $"Npc:{mgr.GetObjects(NodeTypes.ReferenceNpc).Count}");
            if (string.IsNullOrEmpty(node.goName))
            {
                obj.SetActive(node.active);
                return 0;
            }
            var target = BlueprintDictionaryManager.Instance.ResolveReference(node.goName);
            target.SetActive(node.active);
            return 0;
        }
    }
}