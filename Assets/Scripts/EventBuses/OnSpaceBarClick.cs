using EventBuses;
using UnityEngine.InputSystem;

namespace Assets.Scripts.EventBuses
{
    public class OnSpaceBarClick:IEvent
    {
        private InputAction.CallbackContext context;

        public OnSpaceBarClick(InputAction.CallbackContext context)
        {
            this.context = context;
        }
    }
}