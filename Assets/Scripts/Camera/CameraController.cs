using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { Static, Follow, Dialogue }

    [SerializeField] private CameraState initialState = CameraState.Follow;
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset;

    [Header("Dead Zone Settings")]
    [SerializeField] private Vector2 deadZone = new Vector2(3f, 2f);
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private EventBusSO EventBus;

    private Action currentStateBehavior;
    private Vector3 velocity = Vector3.zero;
    private CameraState currentCameraState;

    private void OnEnable()
    {
        EventBus.Subscribe<OnDialogueCameraDataEvent>(DialogueStarted);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe<OnDialogueCameraDataEvent>(DialogueStarted);
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
    }

    public void ChangeCameraState(CameraState newState)
    {
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
            default:
                Debug.Log($"Camera State {newState} does not exist.");
                break;
        }
    }

    private void LateUpdate()
    {
        currentStateBehavior?.Invoke();
    }

    private void DialogueStarted(OnDialogueCameraDataEvent ev)
    {
        ChangeCameraState(CameraState.Dialogue);
        ChangeCameraSettings(ev.DialogueCameraData);
    }

    private void ChangeCameraSettings(DialogueCameraData data)
    {
        Vector3 camPos = transform.position;
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
