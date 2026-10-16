using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerMovementSystem))]
    public class PlayerMain : MonoBehaviour
    {
        [SerializeField] private InputActionsSO inputReader;
        [SerializeField] public LayerMask whatIsClickable;
        
        public PlayerMovementSystem playerMovementSystem;
    
        public Camera cam;
    
        private void Awake()
        {
            cam = Camera.main;
            playerMovementSystem = GetComponent<PlayerMovementSystem>();
        }
    }
}
