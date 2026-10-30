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
        //Listas: Dentro del So
        [SerializeField] private DictionarySO dictionarySo; 
        
        private void OnEnable() => RebuildDictionary();
        //
        public Dictionary<NodeTypes, (List<GameObject>list, Type eventRelayType)> nodeDictionary = new();
        
        //Diccionarios
        public Dictionary <string, RuntimeNodeGroup> triggerDictionary = new ();
        public Dictionary <string, RuntimeNodeGroup> interactDictionary = new ();
        
        void RebuildDictionary()
        {
            nodeDictionary[NodeTypes.Trigger] = (dictionarySo.triggerObjects, typeof(TriggerEnterRelay));
            nodeDictionary[NodeTypes.Interactuable] = (dictionarySo.interactObjects, null);
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
                    triggerDictionary.Add(name, group);
                    break;
                case NodeTypes.Interactuable:
                    interactDictionary.Add(name, group);
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
           
    }
}