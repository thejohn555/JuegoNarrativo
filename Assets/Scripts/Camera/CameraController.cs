using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using System;
using EventBuses;
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

    [Header("Camera Bounds")]
    public bool useCameraBounds = false;
    public Vector3 boundsCenter = Vector3.zero;
    public Vector3 boundsSize = new Vector3(20f, 10f, 0f);

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
        currentCameraState = initialState;
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

    private void EnterDialogueState(DialogueData data)
    {
        isDialogue = true;
        stateBeforeDialogue = currentCameraState;
        cameraPosBeforeDialogue = transform.position;

        if (data.moveCamera && data.targetPosition != null)
        {
            currentTargetPos = data.targetPosition.position + data.cameraOffset;
            currentTargetPos.z = transform.position.z;
        }
        else
        {
            currentTargetPos = transform.position;
        }

        if (data.zoomCamera)
        {
            currentTargetZoom = data.zoom;
        }
        else
        {
            currentTargetZoom = cam.orthographicSize;
        }
        currentSmoothTime = data.smoothTime;

        if (!data.moveCamera && !data.zoomCamera)
        {
            EventBus.Publish(new OnDialogueStartedEvent());
            ChangeCameraState(CameraState.Dialogue);
        }
        else
        {
            ChangeCameraState(CameraState.Transitioning);
        }
    }

    public void ExitDialogueState()
    {
        isDialogue = false;

        if (currentCameraState == CameraState.Dialogue && transform.position == cameraPosBeforeDialogue && cam.orthographicSize == defaultZoom)
        {
            ChangeCameraState(stateBeforeDialogue);
            return;
        }

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
            {
                EventBus.Publish(new OnDialogueStartedEvent());
                ChangeCameraState(CameraState.Dialogue);
            }
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
            Vector3 nextPosition = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);

            if (useCameraBounds)
            {
                float halfHeight = cam.orthographicSize;
                float halfWidth = halfHeight * cam.aspect;

                float minX = boundsCenter.x - (boundsSize.x / 2f) + halfWidth;
                float maxX = boundsCenter.x + (boundsSize.x / 2f) - halfWidth;
                float minY = boundsCenter.y - (boundsSize.y / 2f) + halfHeight;
                float maxY = boundsCenter.y + (boundsSize.y / 2f) - halfHeight;

                if (minX > maxX) minX = maxX = boundsCenter.x;
                if (minY > maxY) minY = maxY = boundsCenter.y;

                nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
                nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
            }

            transform.position = nextPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZone.x, deadZone.y, 0f));

        if (useCameraBounds)
        {
            Gizmos.color = Color.darkOliveGreen;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);

            Camera editorCam = GetComponent<Camera>();
            if (editorCam != null)
            {
                float halfHeight = editorCam.orthographicSize;
                float halfWidth = halfHeight * editorCam.aspect;

                Vector3 centerBoundsSize = new Vector3(
                    Mathf.Max(0, boundsSize.x - (halfWidth * 2f)),
                    Mathf.Max(0, boundsSize.y - (halfHeight * 2f)),
                    0f
                );

                Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
                Gizmos.DrawWireCube(boundsCenter, centerBoundsSize);
            }
        }
    }
}