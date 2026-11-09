using Assets.Scripts.Camera;
using System;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Runtime;
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
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private float zoom = 3f; 
        [SerializeField] private float smoothTime = 0.5f;

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus.Subscribe<OnNPCStateChangedEvent>(HandleStateChange);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
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

            Debug.Log($"[{npcID}] State not found: {ev.NewStateID}");
        }

        public override void Interact(GameObject interactor)
        {
            //Nuevo para los blueprints
            BlueprintsEventManager.Instance.Raise(NodeTypes.InteractableNpc,this.gameObject);
            
            
            DialogueData data = new DialogueData();
            data.targetPosition = transform;

            data.cameraOffset = cameraOffset;
            data.zoom = zoom;
            data.smoothTime = smoothTime;
            data.moveCamera = cameraMove;
            data.zoomCamera = zoomCamera;
            data.isDialogue = true;
            data.textColor = npcTextColor;

            data.speakers = new Dictionary<string, SpeakerData>
            {
                { "Conan", new SpeakerData { transform = interactor.transform, textColor = Color.white } },
                { npcID, new SpeakerData { transform = this.transform, textColor = npcTextColor } }
            };


            if (!HeldItemController.Instance.HasHeldItem)
            {
                Debug.Log("Incio dialogo");
                DialogueManager.Instance.StartDialogue(currentGraph, data);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (transform == null) return;

            Vector3 cameraCenter = transform.position + cameraOffset;
            cameraCenter.z = 0;

            float camHeight = zoom * 2f;
  
            float aspect = Camera.main != null ? Camera.main.aspect : (16f / 9f);
            float camWidth = camHeight * aspect;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraCenter, new Vector3(camWidth, camHeight, 0.1f));
        }
    }
}
