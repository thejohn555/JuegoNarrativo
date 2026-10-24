using System;
using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.EventRelay
{
    public class TriggerEnterRelay : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("[TriggerEnterRelay] Triggered");
            BlueprintsEventManager.Instance.TriggerInvoque(gameObject);
        }
        
    }
}