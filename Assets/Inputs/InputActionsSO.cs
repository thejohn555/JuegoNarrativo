using Assets.Scripts.EventBuses;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject, NewInputActions.IGamePlayActions, NewInputActions.IUIActions
{
    private NewInputActions inputActions;
    private void OnEnable()
    {
        inputActions = new NewInputActions();
        inputActions.Enable();
        SwitchToGameplayMapping();
        EventBus.Instance.Subscribe<OnDialogueStartedEvent>(OnDialogueStarted);
        EventBus.Instance.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);
    }

    private void OnDisable()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.UnSubscribe<OnDialogueStartedEvent>(OnDialogueStarted);
            EventBus.Instance.UnSubscribe<OnDialogueEndedEvent>(OnDialogueEnded);
        }
    }

    private void SwitchToGameplayMapping()
    {
        inputActions.GamePlay.Enable();
        //inputActions.Interacting.Disable();
        inputActions.GamePlay.AddCallbacks(this);
    }
    
    private void SwitchToUIMapping()
    {
        inputActions.UI.Enable();
        inputActions.UI.AddCallbacks(this);
    }

    private void OnDialogueStarted(OnDialogueStartedEvent ev)
    {
        SwitchToUIMapping();
    }

    private void OnDialogueEnded(OnDialogueEndedEvent ev)
    {
        SwitchToGameplayMapping();
    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        EventBus.Instance.Publish(new OnLeftMouseClick(context));
    }

    public void OnNextDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
            EventBus.Instance.Publish(new OnSkipDialogueEvent());
    }
}
