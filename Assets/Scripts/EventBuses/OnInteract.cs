using UnityEngine;

namespace Assets.Scripts.EventBuses
{
    public class OnInteract:IEvent
    {

        private GameObject interactor;
        public OnInteract(GameObject interactor)
        {
            this.interactor = interactor;
        }
    }
}