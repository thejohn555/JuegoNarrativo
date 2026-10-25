using UnityEngine;

namespace Interactives
{
    public class NPC:Interactive
    {
        [SerializeField] private RuntimeDialogueGraph Dialogue;
        public override void Interact(GameObject interactor)
        {
            DialogueManager.Instance.StartDialogue(Dialogue);
        }
    }
}