using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { Static, Follow, Dialogue, Transitioning }

    [SerializeField] private CameraState initialState = CameraState.Follow;
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset;

    [Header("Dead Zone Settings")]
    [SerializeField] private Vector2 deadZone = new Vector2(3f, 2f);
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private EventBusSO EventBus;

    [Header("Test Mode")]
    public Transform testDialogueTarget;

    [SerializeField] private float zoomVelocity = 0.5f;
    private Action currentStateBehavior;
    private Vector3 velocity = Vector3.zero;
    private CameraState currentCameraState;
    private CameraState lastCameraState;
    private CameraState stateBeforeDialogue;
    private Camera cam;
    private float defaultZoom;
    private Vector3 currentTargetPos;
    private Vector3 cameraPosBeforeDialogue;
    private float currentTargetZoom;
    private float currentSmoothTime;
    private bool isDialogue;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        defaultZoom = cam.orthographicSize;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<OnDialogueCameraDataEvent>(DialogueStarted);
        EventBus.Subscribe<OnDialogueEndedEvent>(DialogueEnded);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnDialogueCameraDataEvent>(DialogueStarted);
        EventBus.UnSubscribe<OnDialogueEndedEvent>(DialogueEnded);
    }

    private void Start()
    {
        ChangeCameraState(initialState);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CameraState nextState = (currentStateBehavior != null) ? CameraState.Static : CameraState.Follow;
            ChangeCameraState(nextState);
            Debug.Log($"[CameraController] Estado cambiado a: {nextState}");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (testDialogueTarget == null)
            {
                Debug.LogWarning("¡Asigna un Test Dialogue Target en el inspector!");
                return;
            }

            // Creamos unos datos falsos simulando lo que enviaría el NPC
            DialogueCameraData testData = new DialogueCameraData();
            testData.targetPosition = testDialogueTarget;
            testData.cameraOffset = new Vector3(2f, 1f, 0f);
            testData.zoom = 3f;
            testData.smoothTime = 0.5f;

            Debug.Log("Simulando inicio de diálogo (Tecla T)");
            EnterDialogueState(testData);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Simulando fin de diálogo (Tecla Y)");
            ExitDialogueState();
        }
    }

    public void ChangeCameraState(CameraState newState)
    {
        lastCameraState = currentCameraState;
        currentCameraState = newState;

        switch (newState)
        {
            case CameraState.Static:
                currentStateBehavior = null;
                break;
            case CameraState.Follow:
                currentStateBehavior = FollowState;
                break;
            case CameraState.Dialogue:
                currentStateBehavior = null;
                break;
            case CameraState.Transitioning:
                currentStateBehavior = HandleTransition;
                break;
            default:
                Debug.LogWarning($"Camera State {newState} does not exist.");
                break;
        }
    }

    private void LateUpdate()
    {
        currentStateBehavior?.Invoke();
    }

    private void DialogueStarted(OnDialogueCameraDataEvent ev)
    {
        EnterDialogueState(ev.DialogueCameraData);
    }

    private void DialogueEnded(OnDialogueEndedEvent ev)
    {
        ExitDialogueState();
    }

    private void EnterDialogueState(DialogueCameraData data)
    {
        if (data.targetPosition == null) return;
        isDialogue = true;

        currentTargetPos = data.targetPosition.position + data.cameraOffset;
        currentTargetPos.z = transform.position.z;

        currentTargetZoom = data.zoom;
        currentSmoothTime = data.smoothTime;

        stateBeforeDialogue = currentCameraState;
        cameraPosBeforeDialogue = transform.position;
        ChangeCameraState(CameraState.Transitioning);
    }

    public void ExitDialogueState()
    {
        if (target == null) return;
        isDialogue = false;

        currentTargetPos = cameraPosBeforeDialogue;
        currentTargetPos.z = transform.position.z;

        currentTargetZoom = defaultZoom;
        currentSmoothTime = smoothTime;

        ChangeCameraState(CameraState.Transitioning);
    }

    private void HandleTransition()
    {
        transform.position = Vector3.SmoothDamp(transform.position, currentTargetPos, ref velocity, currentSmoothTime);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, currentTargetZoom, ref zoomVelocity, currentSmoothTime);

        float distance = Vector2.Distance(transform.position, currentTargetPos);
        float zoomDiff = Mathf.Abs(cam.orthographicSize - currentTargetZoom);

        if (distance < 0.01f && zoomDiff < 0.01f)
        {
            transform.position = currentTargetPos;
            cam.orthographicSize = currentTargetZoom;
            if (isDialogue)
                ChangeCameraState(CameraState.Dialogue);
            else
                ChangeCameraState(stateBeforeDialogue);
        }
    }

    private void FollowState()
    {
        if (target == null) return;

        Vector3 camPos = transform.position;
        Vector3 targetPos = target.position;

        float distanceX = targetPos.x - camPos.x;
        float distanceY = targetPos.y - camPos.y;

        float moveX = 0f;
        float moveY = 0f;

        if (Mathf.Abs(distanceX) > (deadZone.x / 2f))
        {
            moveX = distanceX - (Mathf.Sign(distanceX) * (deadZone.x / 2f));
        }

        if (Mathf.Abs(distanceY) > (deadZone.y / 2f))
        {
            moveY = distanceY - (Mathf.Sign(distanceY) * (deadZone.y / 2f));
        }

        if (moveX != 0 || moveY != 0)
        {
            Vector3 desiredPos = new Vector3(camPos.x + moveX, camPos.y + moveY, camPos.z);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZone.x, deadZone.y, 0f));
    }
}
