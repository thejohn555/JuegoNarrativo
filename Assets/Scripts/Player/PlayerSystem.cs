using UnityEngine;

namespace Player
{
    
    public class PlayerSystem : MonoBehaviour
    {
        protected PlayerMain main;
        
        protected virtual void Awake()
        {
            main = transform.root.GetComponent<PlayerMain>();
        }
    }
}