
using EventBuses;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.FridayCode
{
    internal class ObjectTrigger : MonoBehaviour
    {

        protected void ObjectDropped(Action method, string desiredName, string itemName)
        {
            if (desiredName == itemName)
            {
                HeldItemController.Instance.Consume();
                method.Invoke();
                Debug.Log("Something was crafted");
            }
            else
            {
                Debug.Log("item erroneo");
            }
        }
    }
}
