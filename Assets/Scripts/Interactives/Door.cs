using DoorGraph;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactives
{
    public class Door : Interactive
    {
        [field: SerializeField] public int DestinationScenePath { get; private set; }

        public override void Interact(GameObject interactor)
        {
            GameObject puerta = DoorManager.Instance.GetTargetDoor(this.gameObject);
            if (puerta.TryGetComponent(out Door door))
            {
                SceneGameManager.Instance.SwapScene(door.DestinationScenePath);
            }
        }
    }
}
