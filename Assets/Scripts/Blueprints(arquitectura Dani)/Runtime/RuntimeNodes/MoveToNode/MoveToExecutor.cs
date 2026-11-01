using System.Collections;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.MoveToNode
{
    public class MoveToExecutor:BlueprintNodeExecutor<MoveToRuntimeNode>
    {
        protected override int OnExecute(MoveToRuntimeNode node, BlueprintDirector ctx, GameObject obj)
        {

            GameObject target = obj;
            if (!string.IsNullOrEmpty(node.goName))
            {
                target = BlueprintDictionaryManager.Instance.Resolve(NodeTypes.Reference, node.goName);
            }
            if (target == null)
            {
                return 0;  
            }
            ctx.StartCoroutine(Move(node.positionToGo, node.time, target));
            return 0;
        }
        private IEnumerator Move(Vector2 dest, float time,GameObject go)
        {
            Vector3 inicio = go.transform.position;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                go.transform.position = Vector3.Lerp(inicio, dest, t);
                yield return null;
            }
            go.transform.position = dest;
        }
    }
}