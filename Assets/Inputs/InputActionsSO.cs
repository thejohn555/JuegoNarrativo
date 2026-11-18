using Assets.Scripts.EventBuses;
using EventBuses;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.CodingTools;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject, NewInputActions.IGamePlayActions, NewInputActions.IUIActions, NewInputActions.IInventoryActions, NewInputActions.IGlobalActions
{
    [field: SerializeField] public EventBusSO EventBus { get; private set; }
    //input maps
    private NewInputActions inputActions;
    private InputActionMap currentMap;
    private InputActionMap previousMap;
    //Map refs
    private InputActionMap gameplayMap;
    private InputActionMap inventoryMap;
    OnScrollWheelPressed onScrollWheelPressed = new ();
    
    
    
    private void OnEnable()
    {
    }

    public void Inicialize()
    {
        inputActions = new NewInputActions();
        InputSystem.onDeviceChange += OnInputDeviceChange;
        gameplayMap = inputActions.GamePlay;
        inventoryMap = inputActions.Inventory;

        inputActions.GamePlay.SetCallbacks(this);
        inputActions.UI.SetCallbacks(this);
        inputActions.Inventory.SetCallbacks(this);
        inputActions.Global.SetCallbacks(this);

        EventBus.Subscribe<OnDialogueDataEvent>(OnDialogueStarted);
        EventBus.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);
        EventBus.Subscribe<OnInventoryCloseEvent>(OnInventoryClosed);
        EventBus.Subscribe<OnGamePausedEvent>(HandleGamePaused);
        
        SwitchMap(inputActions.GamePlay);
    }

    private void OnDisable()
    {
        if (EventBus != null)
        {
            EventBus.UnSubscribe<OnDialogueDataEvent>(OnDialogueStarted);
            EventBus.UnSubscribe<OnDialogueEndedEvent>(OnDialogueEnded);
            EventBus.UnSubscribe<OnInventoryCloseEvent>(OnInventoryClosed);
            EventBus.UnSubscribe<OnGamePausedEvent>(HandleGamePaused);
        }

        InputSystem.onDeviceChange -= OnInputDeviceChange;
    }

    private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Mouse || currentMap == null)
            return;

        InputAction leftMouseAction = inputActions.GamePlay.LeftMouse;
        if (leftMouseAction.controls.Count > 0)
            return;

        // Mouse can be added after the ScriptableObject enabled its action map.
        currentMap.Disable();
        currentMap.Enable();

       // Debug.Log(
            //$"INPUT DEVICE Mouse {change}: " +
           // $"LeftMouse controls={leftMouseAction.controls.Count}"
       // );
    }

    private void SwitchMap(InputActionMap newMap)
    {
        currentMap?.Disable();
        currentMap = newMap;
        currentMap?.Enable();
        inputActions.Global.Enable();
        Debug.Log($"Mapa cambiado, actual: {currentMap?.ToString()}");
    }

    private void HandleGamePaused(OnGamePausedEvent ev)
    {
        if (ev.isPaused)
        {
            previousMap = currentMap;
            SwitchMap(null);
        }
        else
        {
            SwitchMap(previousMap);
        }
    }

    private void OnDialogueStarted(OnDialogueDataEvent ev)
    {
        if (ev.DialogueData.isDialogue)
        {
            SwitchMap(inputActions.UI);
        }
    }

    private void OnDialogueEnded(OnDialogueEndedEvent ev)
    {
        SwitchMap(inputActions.GamePlay);
    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        //Debug.Log($"INPUT CALLBACK LeftMouse: phase={context.phase}, map={context.action.actionMap.enabled}");
        if (context.started)
            EventBus.Publish(new OnLeftMouseClick());
    }

    public void OnRightMouse(InputAction.CallbackContext context)
    {
        if (context.started)
            EventBus.Publish(new OnRightMouseClick());
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventBus.Publish(new OnSpaceBarClick(context));
        }
        else if(context.canceled)
        {
            EventBus.Publish(new OnSpaceBarClick(context));
        }
    }

    public void OnNextDialogue(InputAction.CallbackContext context)
    {
        //if (context.action.actionMap.name != "UI") return;
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

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
            EventBus.Publish(new OnEscPressedEvent());
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
