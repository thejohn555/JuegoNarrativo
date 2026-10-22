using System;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.EventRelay
{
    public class TriggerEnterRelay : MonoBehaviour
    {
        public event Action<GameObject,Collider> Enter;

        public void OnTriggerEnter(Collider other)
        {
            Enter?.Invoke(gameObject, other);
        }
        
    }
}