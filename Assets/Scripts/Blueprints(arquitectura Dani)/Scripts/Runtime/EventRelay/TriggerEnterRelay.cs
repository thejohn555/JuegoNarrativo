using System;
using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.EventRelay
{
    public class TriggerEnterRelay : MonoBehaviour,IEventRelay
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[Relay] Trigger en '{name}' por '{other.name}'");
            BlueprintsEventManager.Instance.Raise(NodeTypes.Trigger,transform.root.gameObject);
        }
        
    }
}