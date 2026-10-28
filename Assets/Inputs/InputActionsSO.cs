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
    [Header("Inventory Cooldown")]
    [SerializeField]private float cooldownTime;
    private bool cooldown;
    private NewInputActions inputActions;
    private InputActionMap currentMap;
    //Map refs
    private InputActionMap gameplayMap;
    private InputActionMap inventoryMap;
    OnScrollWheelPressed onScrollWheelPressed = new OnScrollWheelPressed();
    
    
    
    private void OnEnable()
    {
        //cooldown var
        cooldown = true;


        inputActions = new NewInputActions();
        inputActions.Enable();
        gameplayMap = inputActions.GamePlay;
        inventoryMap = inputActions.Inventory;

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
        //Debug.Log($"Mapa cambiado, actual: {currentMap}");
        Debug.Log(currentMap?.ToString());
    }

    private void OnDialogueStarted(OnDialogueCameraDataEvent ev)
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
        if (context.started && currentMap == gameplayMap && cooldown)
        {
            SwitchMap(inputActions.Inventory);
            EventBus.Publish(onScrollWheelPressed);
            //Timer.Instance.StartCoroutine(Timer.Instance.Countdown(cooldownTime, cooldown));
           
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
