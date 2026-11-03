using System;
using System.Collections.Generic;
using System.Linq;
using Blueprints_arquitectura_Dani_.EventRelay;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.Managers
{
    public class BlueprintDictionaryManager : MonoBehaviour
    {
        
        public static BlueprintDictionaryManager Instance { get; private set; }
        //Listas
        [SerializeField] private List<GameObject> triggerObjects = new();      // persiste en la escena
        [SerializeField] private List<GameObject> interactObjects = new();
        [SerializeField] private List<GameObject> referenceObjects = new();
        [SerializeField] private List<GameObject> npcObjects = new();
        
        private void OnEnable() => RebuildDictionary();
        //[SerializeField] private DictionarySO dictionarySo; 
        public Dictionary<NodeTypes, (List<GameObject>list, Type eventRelayType)> nodeDictionary = new();
        
        //Diccionarios
        public Dictionary <string, RuntimeNodeGroup> triggerDictionary = new ();
        public Dictionary <string, RuntimeNodeGroup> interactDictionary = new ();
        public Dictionary <string,RuntimeNodeGroup> npcDictionary = new ();
        
        void RebuildDictionary()
        {
            nodeDictionary[NodeTypes.Trigger] = (triggerObjects, typeof(TriggerEnterRelay));
            nodeDictionary[NodeTypes.Interactable] = (interactObjects, null);
            nodeDictionary[NodeTypes.Reference] = (referenceObjects, null);
            nodeDictionary[NodeTypes.InteractableNpc]= (npcObjects, null);
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        
#if UNITY_EDITOR
        public static event System.Action NodesChanged;
#endif
        public void AddToList(GameObject go,NodeTypes type)
        {
            RebuildDictionary();
            if (!nodeDictionary.TryGetValue(type, out var entry))
            {
                Debug.LogWarning($"[Manager] Tipo {type} no registrado");
                return;
            }

            if (nodeDictionary[type].eventRelayType != null)
            {
                var col = go.GetComponentInChildren<Collider2D>();
                if (col == null)
                {
                    Debug.LogWarning($"[Manager] '{go.name}' no tiene Collider2D", go);
                }
                else if (!col.TryGetComponent(entry.eventRelayType, out _))
                {
                    col.gameObject.AddComponent(entry.eventRelayType);
                }
            }

            CleanNullsFromList(entry.list);
            if (!entry.list.Contains(go))
                entry.list.Add(go);
#if UNITY_EDITOR
            NodesChanged?.Invoke();
#endif

// #if UNITY_EDITOR
//             UnityEditor.EditorUtility.SetDirty(this);
//             dictionarySo?.AddName(trigger.name);
// #endif
        }

        public void AddToDictionary(NodeTypes type,string name, RuntimeNodeGroup group)
        {
            switch (type)
            {
                case NodeTypes.Trigger:
                    triggerDictionary[name] = group;
                    break;
                case NodeTypes.Interactable:
                    interactDictionary[name] = group;
                    break;
                case NodeTypes.InteractableNpc:
                    npcDictionary[name] = group;
                    break;
            }
            
        }

        public void CleanNullsFromList(List<GameObject> list)
        { 
            list.RemoveAll(g => g == null);
        }
        
            

        public bool HasTrigger(string key)
        {
            if (!nodeDictionary.TryGetValue(NodeTypes.Trigger, out var entry)) return false;
            foreach (var g in entry.list)
                if (g != null && g.name == key) return true;
            return false;
        }
        
        public List<GameObject> GetObjects(NodeTypes type)
        {
            RebuildDictionary();
            return nodeDictionary.TryGetValue(type, out var entry)
                ? entry.list
                : new List<GameObject>();
        }
        
        public GameObject Resolve(NodeTypes type, string key)
        {
            if (!nodeDictionary.TryGetValue(type, out var entry)) return null;

            foreach (var go in entry.list)
                if (go != null && go.name == key)
                    return go;

            return null;
        }
           
    }
}