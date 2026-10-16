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
                {typeof(ConditionRuntimeNode),new ConditionNodeExecutor()},
            };
        }

        private void Start()
        {
            if (RuntimeGraph == null)
            {
                Debug.LogError("No Graph assigned to StartRuntimeNode");
                return;
            }

            foreach (var node in RuntimeGraph.RuntimeNodes)
            {
                if (node.Nodes.Count == 0)
                    continue;
                var currentNode = node.Nodes[0];
                while (currentNode != null)
                {
                    if (!executors.TryGetValue(currentNode.GetType(), out var executor))
                    {
                        Debug.LogError(
                            $"No executor found for this type: {currentNode.GetType()}");
                        break;
                    }

                    int nextNodeIndex = -1;

                    if (currentNode is StartRuntimeNode startNode)
                    {
                        var startExecutor =
                            (IBlueprintNodeExecutor<StartRuntimeNode>)executor;

                        startExecutor.Execute(startNode, this);

                        if (startNode.NextNodeIndex.Count > 0)
                            nextNodeIndex = startNode.NextNodeIndex[0];
                    }
                    else if (currentNode is ConditionRuntimeNode conditionNode)
                    {
                        var conditionExecutor =
                            (IBlueprintNodeExecutor<ConditionRuntimeNode>)executor;

                        bool result = conditionExecutor.Execute(conditionNode, this);

                        if (conditionNode.NextNodeIndex.Count > 0)
                        {
                            // Provisional: solo funciona si la primera conexión
                            // corresponde al resultado que quieres seguir.
                            if (result)
                                nextNodeIndex = conditionNode.NextNodeIndex[0];
                        }
                    }

                    if (nextNodeIndex < 0)
                        break;

                    if (nextNodeIndex >= node.Nodes.Count)
                    {
                        Debug.LogError($"Índice de nodo inválido: {nextNodeIndex}");
                        break;
                    }

                    currentNode = node.Nodes[nextNodeIndex];
                }
            }
        }
    }
}