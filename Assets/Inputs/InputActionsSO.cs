using Assets.Scripts.EventBuses;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject, NewInputActions.IGamePlayActions, NewInputActions.IUIActions
{
    [field: SerializeField] public EventBusSO EventBus { get; private set; }
    private NewInputActions inputActions;
    private InputActionMap currentMap;
    private void OnEnable()
    {
        inputActions = new NewInputActions();
        inputActions.Enable();

        inputActions.GamePlay.SetCallbacks(this);
        inputActions.UI.SetCallbacks(this);

        EventBus.Subscribe<OnDialogueStartedEvent>(OnDialogueStarted);
        EventBus.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);

        SwitchMap(inputActions.GamePlay);
    }

    private void OnDisable()
    {
        if (EventBus != null)
        {
            EventBus.UnSubscribe<OnDialogueStartedEvent>(OnDialogueStarted);
            EventBus.UnSubscribe<OnDialogueEndedEvent>(OnDialogueEnded);
        }
    }

    private void SwitchMap(InputActionMap newMap)
    {
        currentMap?.Disable();
        currentMap = newMap;
        currentMap?.Enable();
        Debug.Log($"Mapa cambiado, actual: {currentMap}");
    }

    private void OnDialogueStarted(OnDialogueStartedEvent ev)
    {
        SwitchMap(inputActions.UI);
    }

    private void OnDialogueEnded(OnDialogueEndedEvent ev)
    {
        SwitchMap(inputActions.GamePlay);
    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventBus.Publish(new OnLeftMouseClick(context));
        }
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventBus.Publish(new OnSpaceBarClick(context));
        }
    }

    public void OnNextDialogue(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            EventBus.Publish(new OnSkipDialogueEvent());
        }
    }
}
