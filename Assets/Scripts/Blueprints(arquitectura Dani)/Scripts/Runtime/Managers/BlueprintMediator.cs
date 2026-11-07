using System;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintMediator : MonoBehaviour
    {
        public static BlueprintMediator Instance { get; private set; }
        
        public string LastGuidPick { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            BlueprintsEventManager.Instance.OnGetLastGuid += InstanceOnOnGetLastGuid;
        }
        void InstanceOnOnGetLastGuid(string obj)
        {
            LastGuidPick = obj;
        }
    }
}