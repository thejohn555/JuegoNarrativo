using System;
using System.Collections;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime.InteractNode;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.AddItemToInventory;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.DelayNode;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.MoveToNode;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.OnInteractNpcNode;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.SetActive;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.StartDialogue;
using Blueprints_arquitectura_Dani_.Runtime.TriggerNode;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.ChangeNpcState;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.OnItemCollected;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.PlayTimeLine;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.SetInteractable;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.SetPlayerMovement;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.StartBark;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintDirector : MonoBehaviour
    {
        [Header("Graph")]
        [SerializeField]private BlueprintRuntimeGraph[] RuntimeGraph;
        private Dictionary<System.Type, IBlueprintNodeExecutor> executors;
        private Dictionary<(string item, string target), RuntimeNodeGroup> useItemGroups = new();

        public BlueprintDirector Instance { get; private set; }
        public float PendingWait { get; set; }
        private void Awake()
        {
            //Inicializamos el dicionario con el tipo y el ejecutor
            executors = new Dictionary<Type, IBlueprintNodeExecutor>
            {
                {typeof(StartRuntimeNode),new StartNodeExecutor()},
                {typeof(TriggerEnterRuntimeNode),new TriggerNodeExecutor()},
                {typeof(InteractRuntimeNode),new InteractNodeExecutor()},
                {typeof(DelayNodeRuntime),new DelayNodeExecutor()},
                {typeof(SetActiveRuntimeNode),new SetActiveExecutor()},
                {typeof(MoveToRuntimeNode),new MoveToExecutor()},
                {typeof(AddItemToInventoryRuntime),new AddToiInventoryExecutor()},
                {typeof(OnInteractNpcRuntime),new OnInteractNpcExecutor()},
                {typeof(StartDialogueRuntime),new  StartDialogueExecutor()},
                {typeof(ChangeNpcStateRuntime),new ChangeNpcStateExecutor()},
                {typeof(OnItemCollectedRuntime),new OnitemCollectedExecutor()},
                {typeof(SetInteractableRuntime),new  SetInteractableExecutor()},
                {typeof(SetPlayerMovementRuntime),new  SetPlayerMovementExecutor()},
                {typeof(PlayTimelineRuntime),new PlayTimelineExecutor()},
                {typeof(StartBarkRuntime),new StartBarkExecutor()},
                
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
            
            if (RuntimeGraph == null)
            {
                Debug.LogError("No hay ningun grafo asignado");
                return;
            }
            if (BlueprintsEventManager.Instance != null)
                BlueprintsEventManager.Instance.OnEvent += HandleEvent;
            string actualevel = SceneManager.GetActiveScene().name;
            foreach (var RuntimeGraph in RuntimeGraph)
            {
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
                            StartCoroutine(RunFrom(node));
                            break;
                        case TriggerEnterRuntimeNode trigger:
                            if (!BlueprintDictionaryManager.Instance.HasTrigger(trigger.trigger))
                            {
                                Debug.LogWarning($"[Director] No existe el trigger '{trigger.trigger}' en la escena");
                                break;
                            }

                            BlueprintDictionaryManager.Instance.AddToDictionary(NodeTypes.Trigger, trigger.trigger,
                                node);
                            Debug.Log($"[Director] Registrado trigger '{trigger.trigger}'");
                            break;
                        case InteractRuntimeNode interact:
                            BlueprintDictionaryManager.Instance.AddToDictionary(NodeTypes.Interactable,
                                interact.interact, node);
                            break;
                        case OnInteractNpcRuntime npc:
                            Debug.Log($"[Director] Registrando NPC con key '{npc.interact}'");
                            BlueprintDictionaryManager.Instance.AddToDictionary(NodeTypes.InteractableNpc, npc.interact,
                                node);
                            break;
                        case OnItemCollectedRuntime:
                            BlueprintDictionaryManager.Instance.AddToDictionary(NodeTypes.OnItemColleted, "-", node);
                            break;
                    }

                    idx++;
                }
            }
        }

        private IEnumerator RunFrom(RuntimeNodeGroup group,GameObject source = null,int startIndex = 0)
        {
            int index = startIndex;
            
            while (index>=0&&index < group.Nodes.Count)
            {
                var currentNode = group.Nodes[index];
                if (!executors.TryGetValue(currentNode.GetType(), out var executor))
                {
                    Debug.LogError(
                        $"No executor found for this type: {currentNode.GetType()}");
                    yield break;
                }
                else
                {
                    int output = executor.Execute(currentNode, this,source);

                    if (PendingWait > 0f)
                    {
                        float wait = PendingWait;
                        PendingWait = 0f;                    
                        yield return new WaitForSeconds(wait);
                    }
                    if (output < 0 || output >= currentNode.NextNodeIndex.Count)
                        yield break;                              // fin de la cadena

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
                case NodeTypes.Interactable:
                    Run(manager.interactDictionary, obj, "interactuable");
                    break;
                case NodeTypes.InteractableNpc:
                    Run(manager.npcDictionary, obj, "npc");
                    break;
                case  NodeTypes.OnItemColleted:
                    foreach (var group in manager.itemCollectedGroups)
                        StartCoroutine(RunFrom(group,obj));
                    break;
            }
        }
        private void Run(Dictionary<string, RuntimeNodeGroup> dict, GameObject obj, string kind)
        {
            if (dict.TryGetValue(obj.name, out var group))
                StartCoroutine(RunFrom(group, obj));
            else
            {
                Debug.LogWarning($"No hay {kind} '{obj.name}'. Registrados: {string.Join(", ", dict.Keys)}");
                Debug.LogWarning($"No hay {kind} '{obj.name}'. Registrados: {string.Join(", ", dict.Keys)}");
                //Debug.LogError($"No hay ningún {kind} '{obj.name}'");
            }
        }
        
        
    }
}