using System;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintsEventManager :  MonoBehaviour
    {
        public static BlueprintsEventManager Instance { get; private set; }

        public event Action<GameObject> OnTrigger;
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

        public void TriggerInvoque(GameObject gameObject)
        {
            Debug.Log("Evento");
            OnTrigger?.Invoke(gameObject);
        }
    }
}