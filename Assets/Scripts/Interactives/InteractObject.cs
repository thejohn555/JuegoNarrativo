using UnityEngine;

namespace Interactives
{
    public class InteractObject : Interactive
    {

        public override void Interact(GameObject interactor)
        {
            //EventBus.Publish(new OnInteract(interactor));
        }
    }
}
