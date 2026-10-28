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
        [SerializeField] private Sprite coinSprite;
        [Header("item you need to craft name and result item name")]
        [SerializeField] private String entryObjectName;
        [SerializeField] private String resultObjectName;
        [SerializeField] private PlayerMovementSystem playerMovementSystem;

        private String itemName;

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
                this.itemName = itemName;
                StopAllCoroutines();
                playerMovementSystem.SetNewDestination(gameObject.transform.position,1.5f);
                StartCoroutine(randomShit());
                
            }
        }

        

        private IEnumerator randomShit()
        {
            while (!playerMovementSystem.ReacheDestination())
            {
                yield return null;
            }
            ObjectDropped(CoinSpawn, entryObjectName, itemName);
        }

        private void CoinSpawn()
        {
            InventoryManager.Instance.AddItem(resultObjectName, coinSprite);
            Debug.Log($"the object is {resultObjectName}");
        }

    }
}
