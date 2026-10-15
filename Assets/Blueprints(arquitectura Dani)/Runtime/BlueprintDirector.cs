using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintDirector : MonoBehaviour
    {
        [Header("Graph")]
        public BlueprintRuntimeGraph RuntimeGraph;

        private Dictionary<System.Type, object> executors;

        private void Awake()
        {
            //Inicializamos el dicionario con el tipo y el ejecutor
            executors = new Dictionary<Type, object>
            {
                {typeof(StartRuntimeNode),new StartNodeExecutor()},
                {typeof(ConditionRuntimeNode),new ConditionRuntimeNode()},
            };
        }

        private void Start()
        {
            if (RuntimeGraph == null)
            {
                Debug.LogError("No Graph assigned to StartRuntimeNode");
                return;
            }

            var currentNode = RuntimeGraph.RuntimeNodes[0];

            while (currentNode != null)
            {
                if (!executors.TryGetValue(currentNode.GetType(), out var executor))
                {
                    Debug.LogError($"No executor found for this type: {currentNode.GetType()}");
                    break;
                }

                if (currentNode is ConditionRuntimeNode conditionRuntimeNode)
                {
                    var condicionExecutor = (IBlueprintNodeExecutor<ConditionRuntimeNode>)executor;
                    if (condicionExecutor.Execute(conditionRuntimeNode, this) &&
                        conditionRuntimeNode.NextNodeIndex.Count > 0)
                    {
                        currentNode = RuntimeGraph.RuntimeNodes[conditionRuntimeNode.NextNodeIndex[0]];
                    }
                }

                if (currentNode is StartRuntimeNode startNode)
                {
                    var startExecutor = (IBlueprintNodeExecutor<StartRuntimeNode>)executor;
                    startExecutor.Execute(startNode,this);
                    currentNode = startNode.NextNodeIndex.Count > 0
                        ? RuntimeGraph.RuntimeNodes[startNode.NextNodeIndex[0]]
                        : null;
                }
            }
        }
    }
}