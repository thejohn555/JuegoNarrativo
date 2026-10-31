using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.InteractNode
{
    public class InteractNodeExecutor:BlueprintNodeExecutor<InteractRuntimeNode>
    {
        protected override int OnExecute(InteractRuntimeNode node, BlueprintDirector ctx, GameObject obj)
        {
            Debug.Log(node.gameObject);
            if (node.gameObject == null)
            {
                Debug.Log("Sin gameobject asignado");
                return 0;
            }
            else if (BlueprintsEventManager.Instance.IsHoldingPrefab?.Invoke(node.gameObject) == true)
            {
                Debug.Log("IsHoldingPrefab true");
                return 0;
            }
            else
            {
                Debug.Log("IsHoldingPrefab false");
                return -1;
            }
            
        }
    }
}