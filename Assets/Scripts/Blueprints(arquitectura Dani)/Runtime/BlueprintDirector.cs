using System;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime.TriggerNode;
using Unity.GraphToolkit.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintDirector : MonoBehaviour
    {
        [Header("Graph")]
        [SerializeField]private BlueprintRuntimeGraph RuntimeGraph;
        private Dictionary<System.Type, IBlueprintNodeExecutor> executors;

        public BlueprintDirector Instance { get; private set; }
        private void Awake()
        {
            //Inicializamos el dicionario con el tipo y el ejecutor
            executors = new Dictionary<Type, IBlueprintNodeExecutor>
            {
                {typeof(StartRuntimeNode),new StartNodeExecutor()},
                {typeof(ConditionRuntimeNode),new ConditionNodeExecutor()},
                {typeof(TriggerEnterRuntimeNode),new TriggerNodeExecutor()}
            };
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            
            if (RuntimeGraph == null)
            {
                Debug.LogError("No hay ningun grafo asignado");
                return;
            }
            string actualevel = SceneManager.GetActiveScene().name;
            if (actualevel != RuntimeGraph.sceneName)
            {
                Debug.LogError("El nivel asignado y el actual son distintos");
                return;
            }
            foreach (var node in RuntimeGraph.RuntimeNodes)
            {
                if (node.Nodes.Count == 0)
                    continue;
                if (node.Nodes[0] is StartRuntimeNode)
                {
                    RunFrom(node);
                }
                //Implementar si es un nodo de evento
            }
        }

        private void RunFrom(RuntimeNodeGroup group,int startIndex = 0)
        {
            int index = startIndex;
            
            while (index>=0&&index < group.Nodes.Count)
            {
                var currentNode = group.Nodes[index];
                if (!executors.TryGetValue(currentNode.GetType(), out var executor))
                {
                    Debug.LogError(
                        $"No executor found for this type: {currentNode.GetType()}");
                    return;
                }
                else
                {
                    int output = executor.Execute(currentNode, this);

                    if (output < 0 || output >= currentNode.NextNodeIndex.Count)
                        return;                              // fin de la cadena

                    index = currentNode.NextNodeIndex[output]; 
                }
            }
        }

        public void SuscribeToEnvents(RuntimeNode node)
        {
            // Cojemos la varibale de la blackboard asociada 
            //     Comprobamo que tiene el EventRelay si no se lo ponemos
            //     Y nos suscribimos al evento
            // if (node is TriggerEnterRuntimeNode)
            // {
            //     
            //     var go = Blackboard.Get(node.Target);
            //     if(go))
            // }
            //
        }
    }
}