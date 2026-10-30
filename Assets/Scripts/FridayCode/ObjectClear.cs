using Assets.Scripts.EventBuses;
using EventBuses;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.FridayCode
{
    
    internal class ObjectClear : MonoBehaviour
    {
        [SerializeField] private EventBusSO eventBus;
        [SerializeField] private int totalInteractuables = 3;
        int counter = 0;

        private void OnEnable()
        {
            eventBus.Subscribe<OnNotOver>(ClearObjts);
            eventBus.Subscribe<OnObjectCrafted>(Reset);
        }
        private void OnDisable()
        {
            eventBus.UnSubscribe<OnNotOver>(ClearObjts);
            eventBus.UnSubscribe<OnObjectCrafted>(Reset);
        }

        private void ClearObjts(OnNotOver args)
        {
            
            counter++;
            if (counter == totalInteractuables)
            {
                HeldItemController.Instance.ClearState();
                counter = 0;
            }

        }
        private void Reset(OnObjectCrafted args)
        {
            counter = 0;
        }

    }
}
