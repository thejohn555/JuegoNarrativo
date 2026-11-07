using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.SetInteractable
{
    public class SetInteractableExecutor:BlueprintNodeExecutor<SetInteractableRuntime>
    {
        protected override int OnExecute(SetInteractableRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            Debug.Log($"[SetInteractable] goNome='{node.goNome}'");
            GameObject go;

            if (string.IsNullOrEmpty(node.goNome))
            {
                go = obj;
            }
            else
            {
                go = BlueprintDictionaryManager.Instance.ResolveReference(node.goNome);
            }
            if (go == null)
            {
                Debug.LogWarning($"[SetInteractable] No encuentro '{node.goNome}'");
                return -1;
            }

            BlueprintsEventManager.Instance.SetInteractable?.Invoke(go, node.active);
            return 0;
        }
    }
}