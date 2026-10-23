using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMovementSystem))]
    public class PlayerMain : MonoBehaviour
    {
        public EventBusSO  EventBus;
        [SerializeField] private InputActionsSO inputReader;
        [SerializeField] public LayerMask whatIsClickable;

        public PlayerMovementSystem MovementSystem { get; private set; }

        public Camera Cam { get; private set; }

        private void Awake()
        {
            EventBus=inputReader.EventBus;
            Cam = Camera.main;
            MovementSystem = GetComponent<PlayerMovementSystem>();
        }
    }
}
