using Assets.Scripts.Camera;
using UnityEngine;

namespace Interactives
{
    public class NPC:Interactive
    {
        [SerializeField] private RuntimeDialogueGraph Dialogue;
        [Header("Camera Data")]
        [SerializeField] private Transform testDialogueTarget;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1f, 0f); // Offset de la cámara
        [SerializeField] private float zoom = 3f; // Tamaño de la cámara
        [SerializeField] private float smoothTime = 0.5f;
        [SerializeField] private Vector2 dialogueTextOffset = new Vector2(0f, -200f); // Posición UI
        public override void Interact(GameObject interactor)
        {
            DialogueCameraData testData = new DialogueCameraData();
            testData.targetPosition = testDialogueTarget;

            testData.cameraOffset = cameraOffset;
            testData.zoom = zoom;
            testData.smoothTime = smoothTime;
            testData.dialogueTextOffset = dialogueTextOffset;

            DialogueManager.Instance.StartDialogue(Dialogue, testData);
        }
    }
}