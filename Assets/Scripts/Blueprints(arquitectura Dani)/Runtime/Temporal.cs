using System;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class Temporal : MonoBehaviour
    {
        private void Start()
        {
            BlueprintDictionaryManager.Instance.AddTriggerToList(gameObject);
        }
    }
}