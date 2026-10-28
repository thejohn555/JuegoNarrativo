using System;
using System.Collections.Generic;
using System.Linq;
using Blueprints_arquitectura_Dani_.EventRelay;
using Codice.Client.Common.TreeGrouper;
using Codice.CM.WorkspaceServer.DataStore;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.Managers
{
    public class BlueprintDictionaryManager : MonoBehaviour
    {
        
        public static BlueprintDictionaryManager Instance { get; private set; }
        //Listas
        [SerializeField] private List<GameObject> triggerObjects = new();      // persiste en la escena
        [SerializeField] private List<GameObject> interactObjects = new();
        
        private void OnEnable() => RebuildDictionary();
        //[SerializeField] private DictionarySO dictionarySo;          
        public Dictionary <string, RuntimeNodeGroup> triggerDictionary = new ();
        public Dictionary<NodeTypes, (List<GameObject>list, Type eventRelayType)> nodeDictionary = new();
        void RebuildDictionary()
        {
            nodeDictionary[NodeTypes.Trigger] = (triggerObjects, typeof(TriggerEnterRelay));
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
            var col = go.GetComponentInChildren<Collider2D>();
            if (col == null)
            {
                Debug.LogWarning($"[Manager] '{go.name}' no tiene Collider2D", go);
            }
            else if (!col.TryGetComponent(entry.eventRelayType, out _))
            {
                col.gameObject.AddComponent(entry.eventRelayType);
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

        public void AddTriggerToDictionary(string trigger, RuntimeNodeGroup group)
        {
            triggerDictionary.Add(trigger, group);
        }

        public void CleanNullsFromList(List<GameObject> List)
        {
            foreach (var g in List)
            {
                if (g == null)
                {
                    List.Remove(g);
                }
            }
        }

        public bool HasTrigger(string key)
        {
            if (!nodeDictionary.TryGetValue(NodeTypes.Trigger, out var entry)) return false;
            foreach (var g in entry.list)
                if (g != null && g.name == key) return true;
            return false;
        }
           
    }
}