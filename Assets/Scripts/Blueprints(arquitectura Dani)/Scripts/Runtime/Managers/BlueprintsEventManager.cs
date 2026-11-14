using System;
using Assets.Scripts.Camera;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintsEventManager :  MonoBehaviour
    {
        public static BlueprintsEventManager Instance { get; private set; }
        
        public event Action<NodeTypes, GameObject> OnEvent;
        public event Action<string> OnGetLastGuid;
        public event Action<GameObject> OnStartBark;
        
        
        public Func<string, bool> IsHoldingItem;
        public Func<bool> ConsumeItem;
        public Func<string,Sprite,string, bool> AddItemToInventory;
        public Action<GameObject> StartNpcDialogue;
        public Action<string, string> ChangeNpcState;
        public Action<GameObject, bool> SetInteractable;
        public Action<bool> SetMovement;
       
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Raise(NodeTypes type, GameObject source)
        {
            OnEvent?.Invoke(type, source);
        }

        public void GetLastGuid(string guid)
        {
            OnGetLastGuid?.Invoke(guid);
        }

        public void StartBark(GameObject source)
        {
            Debug.Log($"[EventManager] Lanzando Bark para: {source.name}");
            Debug.Log($"[EventManager] Suscriptores: {OnStartBark?.GetInvocationList().Length ?? 0}");
            OnStartBark?.Invoke(source);
        }
    }
}