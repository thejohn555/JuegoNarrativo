using Player;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMain : MonoBehaviour
{
    
    [SerializeField] private InputActionsSO inputReader;
    [SerializeField] public LayerMask whatIsClickable;
    
    public NavMeshAgent Agent { get; private set; }
    
    public PlayerMovementSystem playerMovementSystem;
    
    public Camera cam;
    
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        playerMovementSystem = GetComponent<PlayerMovementSystem>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
