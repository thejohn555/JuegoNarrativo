using Assets.Scripts.EventBuses;
using EventBuses;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.CodingTools;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject, NewInputActions.IGamePlayActions, NewInputActions.IUIActions, NewInputActions.IInventoryActions
{
    [field: SerializeField] public EventBusSO EventBus { get; private set; }
    //input maps
    private NewInputActions inputActions;
    private InputActionMap currentMap;
    //Map refs
    private InputActionMap gameplayMap;
    private InputActionMap inventoryMap;
    OnScrollWheelPressed onScrollWheelPressed = new OnScrollWheelPressed();
    
    
    
    private void OnEnable()
    {
        inputActions = new NewInputActions();
        inputActions.Enable();
        gameplayMap = inputActions.GamePlay;
        inventoryMap = inputActions.Inventory;

        inputActions.GamePlay.SetCallbacks(this);
        inputActions.UI.SetCallbacks(this);
        inputActions.Inventory.SetCallbacks(this);

        EventBus.Subscribe<OnDialogueDataEvent>(OnDialogueStarted);
        EventBus.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);
        EventBus.Subscribe<OnInventoryCloseEvent>(OnInventoryClosed);
        
        SwitchMap(inputActions.GamePlay);
    }

    private void OnDisable()
    {
        if (EventBus != null)
        {
            EventBus.UnSubscribe<OnDialogueDataEvent>(OnDialogueStarted);
            EventBus.UnSubscribe<OnDialogueEndedEvent>(OnDialogueEnded);
            EventBus.UnSubscribe<OnInventoryCloseEvent>(OnInventoryClosed);
        }
    }

    private void SwitchMap(InputActionMap newMap)
    {
        currentMap?.Disable();
        currentMap = newMap;
        currentMap?.Enable();
        //Debug.Log($"Mapa cambiado, actual: {currentMap}");
        Debug.Log(currentMap?.ToString());
    }

    private void OnDialogueStarted(OnDialogueDataEvent ev)
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
            EventBus.Publish(new OnLeftMouseClick());
        }
    }

    public void OnRightMouse(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventBus.Publish(new OnRightMouseClick());
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
        if (context.started && currentMap == gameplayMap)
        {
            SwitchMap(inputActions.Inventory);
            EventBus.Publish(onScrollWheelPressed);
        }  
    }
    
    private void OnInventoryClosed(OnInventoryCloseEvent ev)
    {
        // Solo si realmente estamos en el mapa de inventario
        if (currentMap == inventoryMap)
        {
            SwitchMap(inputActions.GamePlay);
        }
    }

    #region inventoryInputMap
    
    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if (context.started && currentMap == inventoryMap)
        {
            SwitchMap(inputActions.GamePlay);
            EventBus.Publish(onScrollWheelPressed);
            
        } 
    }
    #endregion
}
