using Assets.Scripts.EventBuses;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject, NewInputActions.IGamePlayActions, NewInputActions.IUIActions, NewInputActions.IInventoryActions
{
    [field: SerializeField] public EventBusSO EventBus { get; private set; }
    private NewInputActions inputActions;
    private InputActionMap currentMap;

    //events to avoid creating them multiple times
    OnTabPressed onTabPressed = new OnTabPressed();
    private void OnEnable()
    {
        inputActions = new NewInputActions();
        inputActions.Enable();

        inputActions.GamePlay.SetCallbacks(this);
        inputActions.UI.SetCallbacks(this);
        inputActions.Inventory.SetCallbacks(this);

        EventBus.Subscribe<OnDialogueCameraDataEvent>(OnDialogueStarted);
        EventBus.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);

        SwitchMap(inputActions.GamePlay);
    }

    private void OnDisable()
    {
        if (EventBus != null)
        {
            EventBus.UnSubscribe<OnDialogueCameraDataEvent>(OnDialogueStarted);
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

    private void OnDialogueStarted(OnDialogueCameraDataEvent ev)
    {
        SwitchMap(inputActions.UI);
    }

    private void OnDialogueEnded(OnDialogueEndedEvent ev)
    {
        SwitchMap(inputActions.GamePlay);
    }

    public void OnNextDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventBus.Publish(new OnSkipDialogueEvent());
        }
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
        if (context.action.actionMap.name != "UI") return;
        if (context.started)
        {
            EventBus.Publish(new OnSkipDialogueEvent());
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventBus.Publish(onTabPressed);
            SwitchMap(inputActions.Inventory);
            Debug.Log("a");
        }  
    }

    #region inventoryInputMap


    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventBus.Publish(onTabPressed);
            SwitchMap(inputActions.GamePlay);
            Debug.Log("b");
        } 
    }
    #endregion






}
