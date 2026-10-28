using Assets.Scripts.Camera;
using System;
using System.Collections.Generic;
using EventBuses;
using UnityEngine;

namespace Interactives
{
    [Serializable]
    public struct DialogueState
    {
        public string StateID;
        public RuntimeDialogueGraph DialogueGraph;
    }

    public class NPC:Interactive
    {
        [Header("NPC Id")]
        public string npcID;

        [Header("Dialogues")]
        [SerializeField] private RuntimeDialogueGraph currentGraph;
        [SerializeField] private Color npcTextColor = Color.white;
        [SerializeField] private List<DialogueState> alternativeStates;
        [SerializeField] bool cameraMove;
        [SerializeField] bool zoomCamera;

        [Header("Events")]
        [SerializeField] private EventBusSO EventBus;

        [Header("Camera Data")]
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1f, 0f); // Offset de la cámara
        [SerializeField] private float zoom = 3f; // Tamaño de la cámara
        [SerializeField] private float smoothTime = 0.5f;
        [SerializeField] private Vector2 dialogueTextOffset = new Vector2(0f, -200f); // Posición UI

        private void OnEnable()
        {
            EventBus.Subscribe<OnNPCStateChangedEvent>(HandleStateChange);
        }
        private void OnDisable()
        {
            EventBus.UnSubscribe<OnNPCStateChangedEvent>(HandleStateChange);
        }

        private void HandleStateChange(OnNPCStateChangedEvent ev)
        {
            if (ev.TargetNpcID != npcID) return;

            foreach (var state in alternativeStates)
            {
                if (state.StateID == ev.NewStateID)
                {
                    currentGraph = state.DialogueGraph;
                    Debug.Log($"[{npcID}] Dialogue graph changed to state: {ev.NewStateID}");
                    return;
                }
            }

            Debug.LogWarning($"[{npcID}] State not found: {ev.NewStateID}");
        }

        public override void Interact(GameObject interactor)
        {
            DialogueData testData = new DialogueData();
            testData.targetPosition = transform;

            testData.cameraOffset = cameraOffset;
            testData.zoom = zoom;
            testData.smoothTime = smoothTime;
            testData.dialogueTextOffset = dialogueTextOffset;
            testData.moveCamera = cameraMove;
            testData.zoomCamera = zoomCamera;
            testData.isDialogue = true;
            testData.textColor = npcTextColor;

            DialogueManager.Instance.StartDialogue(currentGraph, testData);
        }

        protected override void OnDrawGizmosSelected()
        {
            if (transform == null) return;

            Vector3 cameraCenter = transform.position + cameraOffset;
            cameraCenter.z = 0;

            float camHeight = zoom * 2f;
  
            float aspect = Camera.main != null ? Camera.main.aspect : (16f / 9f);
            float camWidth = camHeight * aspect;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraCenter, new Vector3(camWidth, camHeight, 0.1f));

            float pixelsToUnits = camHeight / Screen.height;
            Vector3 uiWorldOffset = new Vector3(dialogueTextOffset.x * pixelsToUnits, dialogueTextOffset.y * pixelsToUnits, 0f);

            Vector3 uiCenter = cameraCenter + uiWorldOffset;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(uiCenter, new Vector3(camWidth * 0.4f, camHeight * 0.2f, 0.1f));

            Gizmos.color = new Color(1f, 0.9f, 0f, 0.5f);
            Gizmos.DrawLine(transform.position, uiCenter);
        }
    }
}
