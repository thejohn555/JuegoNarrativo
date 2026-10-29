using System;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime.InteractNode;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
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

        private void OnDisable()
        {
            if (BlueprintsEventManager.Instance != null)
                BlueprintsEventManager.Instance.OnEvent -= HandleEvent;
        }

        private void Start()
        {
            if (BlueprintsEventManager.Instance != null)
                BlueprintsEventManager.Instance.OnEvent += HandleEvent;
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
                int idx = 0;
                switch (node.Nodes[idx])
                {
                   case StartRuntimeNode:
                       RunFrom(node);
                       break;
                   case TriggerEnterRuntimeNode trigger:
                       if (!BlueprintDictionaryManager.Instance.HasTrigger(trigger.trigger))
                       {
                           Debug.LogWarning($"[Director] No existe el trigger '{trigger.trigger}' en la escena");
                           break;
                       }
                       BlueprintDictionaryManager.Instance.AddToDictionary(NodeTypes.Trigger,trigger.trigger, node);
                       Debug.Log($"[Director] Registrado trigger '{trigger.trigger}'");
                       break;
                   case InteractRuntimeNode  interact:
                       BlueprintDictionaryManager.Instance.AddToDictionary(NodeTypes.Interactuable, interact.interact, node);
                       break;
                }
                idx++;
            }   
        }

        private void RunFrom(RuntimeNodeGroup group,GameObject source = null,int startIndex = 0)
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
                    int output = executor.Execute(currentNode, this,source);

                    if (output < 0 || output >= currentNode.NextNodeIndex.Count)
                        return;                              // fin de la cadena

                    index = currentNode.NextNodeIndex[output]; 
                }
            }
        }
        
        private void HandleEvent(NodeTypes type, GameObject obj)
        {
            var manager = BlueprintDictionaryManager.Instance;
            switch (type)
            {
                case NodeTypes.Trigger:
                    Run(manager.triggerDictionary, obj, "trigger");
                    break;
                case NodeTypes.Interactuable:
                    Run(manager.interactDictionary, obj, "interactuable");
                    break;
            }
        }
        private void Run(Dictionary<string, RuntimeNodeGroup> dict, GameObject obj, string kind)
        {
            if (dict.TryGetValue(obj.name, out var group))
                RunFrom(group, obj);
            else
                Debug.LogError($"No hay ningún {kind} '{obj.name}'");
        }
    }
}