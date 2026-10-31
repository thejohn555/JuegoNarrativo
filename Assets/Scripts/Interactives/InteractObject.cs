using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Interactives
{
    public class InteractObject : Interactive
    {

        public override void Interact(GameObject interactor)
        {
            if (BlueprintsEventManager.Instance == null) return;
                BlueprintsEventManager.Instance.Raise(NodeTypes.Interactable, gameObject);
        }
    }
}
