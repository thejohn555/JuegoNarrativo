using UnityEngine;
using UnityEngine.InputSystem;

public class OnLeftMouseClick : IEvent
{
    private InputAction.CallbackContext context;

    public OnLeftMouseClick(InputAction.CallbackContext context)
    {
        this.context = context;
    }
}
