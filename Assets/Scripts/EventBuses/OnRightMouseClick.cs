using UnityEngine.InputSystem;

namespace Assets.Scripts.EventBuses
{
    public class OnRightMouseClick:IEvent
    {
        
        private InputAction.CallbackContext context;

        public OnRightMouseClick(InputAction.CallbackContext context)
        {
            this.context = context;
        }
        
    }
}