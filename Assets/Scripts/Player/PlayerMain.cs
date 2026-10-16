using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    
    [SerializeField] private InputActionsSO inputReader;
    [SerializeField] private LayerMask whatIsClickable;
    
    private Camera cam;
    
    private void Awake()
    {
        // Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // cam = Camera.main;
        // playerNavMovementSystem = GetComponent<PlayerNavMovementSystem>();
        // attackSystem = GetComponent<PlayerAttackSystem>();
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
