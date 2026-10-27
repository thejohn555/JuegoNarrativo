using System.Collections.Generic;
using System.Linq;
using Blueprints_arquitectura_Dani_.EventRelay;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.Managers
{
    public class BlueprintDictionaryManager : MonoBehaviour
    {
        
        public static BlueprintDictionaryManager Instance { get; private set; }
        [SerializeField] private DictionarySO dictionarySo;          
        public List<GameObject> triggerList = new();
        public Dictionary <string, RuntimeNodeGroup> triggerDictionary = new ();
        
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
        public void AddTriggerToList(GameObject trigger)
        {
            if (!trigger.TryGetComponent<TriggerEnterRelay>(out var eventRelay))
            {
                trigger.AddComponent<TriggerEnterRelay>();
            }

            if (!triggerList.Contains(trigger))
            {
                triggerList.Add(trigger);
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            if (dictionarySo != null)
                dictionarySo.AddName(trigger.name);
            else
                Debug.LogWarning("[Manager] Falta asignar el DictionarySO", this);
#endif
        }

        public void AddTriggerToDictionary(string trigger, RuntimeNodeGroup group)
        {
            triggerDictionary.Add(trigger, group);
        }

        public bool HasTrigger(string key)
        {
            foreach (var g in triggerList)
            {
                if (g != null && g.name == key)
                    return true;
            }
            return false;
        }
           
    }
}