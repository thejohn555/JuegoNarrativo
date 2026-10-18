using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.EventBuses
{
    public class OnSkipDialogueEvent : IEvent
    {
        public InputAction.CallbackContext Context { get; private set; }

        public OnSkipDialogueEvent(InputAction.CallbackContext context)
        {
            Context = context;
        }
    }
}