using EventBuses;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.FridayCode
{
    internal class GamblingTable : ObjectTrigger, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private EventBusSO eventBus;
        [SerializeField] private Sprite sprite;
        [Header("item you need to craft name and result item name")]
        [SerializeField] private String entryObjectName;
        [SerializeField] private String resultObjectName;
        [SerializeField] protected PlayerMovementSystem playerMovementSystem;

       
        
        private String itemName;

        protected bool isOver;

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
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOver = false;
        }

        protected virtual void Action(string itemName)
        {
            if (isOver)
            {
                this.itemName = itemName;
                StopAllCoroutines();
                playerMovementSystem.SetNewDestination(gameObject.transform.position,1.5f);
                StartCoroutine(RandomShit(CoinSpawn));
                
            }
            else
            {
                HeldItemController.Instance.ClearState();
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
