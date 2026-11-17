using System;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintMediator : MonoBehaviour
    {
        public static BlueprintMediator Instance { get; private set; }
        
        public string LastGuidPick { get; private set; }
        public string LastNpcState { get;private set; }
        private BlueprintsEventManager subscribedEventManager;

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
            TrySubscribe();
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void TrySubscribe()
        {
            BlueprintsEventManager eventManager = BlueprintsEventManager.Instance;
            if (eventManager == null || subscribedEventManager == eventManager)
                return;

            subscribedEventManager = eventManager;
            subscribedEventManager.OnGetLastGuid += InstanceOnOnGetLastGuid;
        }

        private void OnDisable()
        {
            if (subscribedEventManager == null)
                return;

            subscribedEventManager.OnGetLastGuid -= InstanceOnOnGetLastGuid;
            subscribedEventManager = null;
        }
        void InstanceOnOnGetLastGuid(string obj)
        {
            LastGuidPick = obj;
        }

        public void GetLastNpcState(string state)
        {
            LastNpcState = state;
        }
    }
}
