using Assets.Scripts.EventBuses;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject,NewInputActions.IGamePlayActions
{
    private NewInputActions inputActions;
    private void OnEnable()
    {
        inputActions = new NewInputActions();
        inputActions.Enable();
        SwitchToGameplayMapping();
    }
    
    private void SwitchToGameplayMapping()
    {
        inputActions.GamePlay.Enable();
        //inputActions.Interacting.Disable();
        inputActions.GamePlay.AddCallbacks(this);
    }
    
    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        EventBus.Instance.Publish(new OnLeftMouseClick(context));
    }

    public void OnSkipDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
            EventBus.Instance.Publish(new OnSkipDialogueEvent(context));
    }
}
