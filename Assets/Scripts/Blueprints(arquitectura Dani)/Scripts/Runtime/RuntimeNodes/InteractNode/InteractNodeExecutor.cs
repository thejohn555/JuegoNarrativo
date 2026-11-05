using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.InteractNode
{
    public class InteractNodeExecutor:BlueprintNodeExecutor<InteractRuntimeNode>
    {
        protected override int OnExecute(InteractRuntimeNode node, BlueprintDirector ctx, GameObject obj)
        {
            var mgr = BlueprintsEventManager.Instance;
            Debug.Log($"[InteractExec] requiredGuid del nodo: '{node.requiredGuid}'");
            Debug.Log($"[InteractExec] ¿IsHoldingItem conectado? {mgr.IsHoldingItem != null}");
            
            if (string.IsNullOrEmpty(node.requiredGuid))
            {
                Debug.Log("Sin item requerido");
                return 0;
            }
            else if (BlueprintsEventManager.Instance.IsHoldingItem?.Invoke(node.requiredGuid) == true)
            {
                Debug.Log("IsHoldingItem true");
                if (node.destroy)
                {
                    BlueprintsEventManager.Instance.ConsumeItem();
                }
                return 0;
            }
            else
            {
                Debug.Log("IsHoldingItem false");
                return -1;
            }
            
        }
    }
}