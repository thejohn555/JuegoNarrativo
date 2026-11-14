using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.EventRelay
{
    public class InteractableSwitch:MonoBehaviour
    {
        private void Start()
        {
            
            if (BlueprintsEventManager.Instance != null)
                BlueprintsEventManager.Instance.SetInteractable = Set;
        }

        private static void Set(GameObject go, bool active)
        {
            Debug.Log($"[Switch] Set {go.name} -> {active}");
            if (go.TryGetComponent<Interactives.Interactive>(out var interactive))
                interactive.SetInteractable(active);
        }
    }
}