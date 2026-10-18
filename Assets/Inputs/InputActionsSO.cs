using Assets.Scripts.EventBuses;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputActionsSO", menuName = "Scriptable Objects/InputActionsSO")]
public class InputActionsSO : ScriptableObject, NewInputActions.IGamePlayActions, NewInputActions.IUIActions
{
    private NewInputActions inputActions;
    private InputActionMap currentMap;
    private void OnEnable()
    {
        inputActions = new NewInputActions();
        inputActions.Enable();

        inputActions.GamePlay.SetCallbacks(this);
        inputActions.UI.SetCallbacks(this);

        EventBus.Instance.Subscribe<OnDialogueStartedEvent>(OnDialogueStarted);
        EventBus.Instance.Subscribe<OnDialogueEndedEvent>(OnDialogueEnded);

        SwitchMap(inputActions.GamePlay);
    }

    private void OnDisable()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.UnSubscribe<OnDialogueStartedEvent>(OnDialogueStarted);
            EventBus.Instance.UnSubscribe<OnDialogueEndedEvent>(OnDialogueEnded);
        }
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static void AutoInitialize()
    //{
    //    //carga el ScriptableObject desde la carpeta Resources al arrancar el juego
    //    Resources.Load<InputActionsSO>("InputActionsSO");
    //}

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
        EventBus.Instance.Publish(new OnLeftMouseClick(context));
    }

    public void OnNextDialogue(InputAction.CallbackContext context)
    {
        Debug.Log($"[InputActionsSO] Callback OnNextDialogue detectada. Fase: {context.phase}");

        if (context.performed)
        {
            Debug.Log("[InputActionsSO] ¡Publicando OnSkipDialogueEvent!");
            EventBus.Instance.Publish(new OnSkipDialogueEvent());
        }
    }
}
