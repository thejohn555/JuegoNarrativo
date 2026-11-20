using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;

namespace Interactives
{
    public class InteractObject : Interactive
    {

        public override void Interact(GameObject interactor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            if (BlueprintsEventManager.Instance == null) return;
                BlueprintsEventManager.Instance.Raise(NodeTypes.Interactable, gameObject);
        }
    }
}
