using System;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintsEventManager :  MonoBehaviour
    {
        public static BlueprintsEventManager Instance { get; private set; }

        public event Action<NodeTypes, GameObject> OnEvent;
        public Func<string, bool> IsHoldingPrefab;
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Raise(NodeTypes type, GameObject source)
        {
            OnEvent?.Invoke(type, source);
        }

       
    }
}