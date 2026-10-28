using Assets.Scripts.EventBuses;
using EventBuses;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.FridayCode
{
    internal class GamblingTable : ObjectTrigger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private EventBusSO eventBus;
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private String desiredObjectName;

        private bool isOver;

        private void OnEnable()
        {
            HeldItemController.Instance.OnItemReleased += Action;
        }
        private void OnDisable()
        {
            HeldItemController.Instance.OnItemReleased -= Action;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOver = true;
            Debug.Log("You can drop the object");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOver = false;
            Debug.Log("You can no longer drop the object");
        }

        private void Action(string itemName)
        {
            if (isOver)
            {
                ObjectDropped(CoinSpawn, desiredObjectName, itemName);
            }
        }

        private void CoinSpawn()
        {
            InventoryManager.Instance.AddItem(desiredObjectName, coinSprite);
        }

    }
}
