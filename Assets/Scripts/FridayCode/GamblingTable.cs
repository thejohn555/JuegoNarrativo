using Assets.Scripts.EventBuses;
using EventBuses;
using Interactives;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.FridayCode
{
    internal class GamblingTable : ObjectTrigger, IPointerEnterHandler, IPointerExitHandler
    {
        
        [SerializeField] private Sprite sprite;
        [Header("item you need to craft name and result item name")]
        [SerializeField] private String entryObjectName;
        [SerializeField] private String resultObjectName;
        [SerializeField] protected PlayerMovementSystem playerMovementSystem;
        [SerializeField] protected float interactionDistance = 1.5f;
        protected OnNotOver onNotOver;
        



        protected String itemName;

        protected bool isOver;

        private void OnEnable()
        {
            HeldItemController.Instance.OnItemReleased += RightClickAction;
        }
        private void OnDisable()
        {
            HeldItemController.Instance.OnItemReleased -= RightClickAction;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOver = false;
        }

        protected virtual void RightClickAction(string itemName)
        {
            if (isOver)
            {
                this.itemName = itemName;
                StopAllCoroutines();
                playerMovementSystem.SetNewDestination(gameObject.transform.position,interactionDistance);
                StartCoroutine(RandomShit(CoinSpawn));
                
            }
            else
            {
                eventBus.Publish(onNotOver);
            }
            
        }

        

        protected IEnumerator RandomShit(Action method)
        {
            while (!playerMovementSystem.ReacheDestination())
            {
                yield return null;
            }
            ObjectDropped(method, entryObjectName, itemName);
        }

        private void CoinSpawn()
        {
            InventoryManager.Instance.AddItem(resultObjectName, sprite);
            Debug.Log($"the object is {resultObjectName}");
        }
        

    }
}
