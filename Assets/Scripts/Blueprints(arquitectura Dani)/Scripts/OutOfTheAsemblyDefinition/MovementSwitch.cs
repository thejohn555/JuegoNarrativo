using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.EventRelay
{
    public class MovementSwitch:MonoBehaviour
    {
        [SerializeField] private GameObject player;
        private void OnEnable()
        {
            if (BlueprintsEventManager.Instance != null)
                BlueprintsEventManager.Instance.SetMovement = Set;
        }

        public void Set(bool state)
        {
            if (player.TryGetComponent<PlayerMovementSystem>(out var playerMovementSystem))
            {
                playerMovementSystem.enabled = state;
            }
        }
    }
}