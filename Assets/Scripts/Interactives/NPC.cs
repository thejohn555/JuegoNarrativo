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
        [SerializeField] private RuntimeDialogueGraph barkGraph;
        [HideInInspector]public RuntimeDialogueGraph CurrentGraph => currentGraph;//Nuevo
        [SerializeField] private Color npcTextColor = Color.white;
        [SerializeField] private List<DialogueState> alternativeStates;
        [SerializeField] bool cameraMove;
        [SerializeField] bool zoomCamera;

        [Header("Events")]
        [SerializeField] private EventBusSO EventBus;

        [field: Header("Camera Data")]
        [SerializeField] private Vector3 cameraOffset  = new Vector3(0f, 1f, 0f);
        [SerializeField] public float zoom = 3f;
        [SerializeField] public float smoothTime  = 0.5f;

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus.Subscribe<OnNPCStateChangedEvent>(HandleStateChange);
            BlueprintsEventManager.Instance.OnStartBark += Handelbark;
            
        }

        private void Handelbark(GameObject obj)
        {
            if (obj != this.gameObject) return;
            TriggerBark();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus.UnSubscribe<OnNPCStateChangedEvent>(HandleStateChange);
            BlueprintsEventManager.Instance.OnStartBark -= Handelbark;
        }

        private void HandleStateChange(OnNPCStateChangedEvent ev)
        {
            // Debug.Log("Handeling");
            // Debug.Log($"Target NpcId: {ev.TargetNpcID}, Npc id:{npcID}");
            if (ev.TargetNpcID != npcID) return;

            foreach (var state in alternativeStates)
            {
                if (state.StateID == ev.NewStateID)
                {
                    currentGraph = state.DialogueGraph;
                    Debug.LogError($"[{npcID}] Dialogue graph changed to state: {ev.NewStateID}");
                    return;
                }
            }

            Debug.Log($"[{npcID}] State not found: {ev.NewStateID}");
        }

        public override void Interact(GameObject interactor)
        {
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
            //Nuevo para los blueprints
            BlueprintsEventManager.Instance.Raise(NodeTypes.InteractableNpc,this.gameObject);
        }

        [ContextMenu("Testear Bark (Monólogo)")]
        public void TestBark()
        {
            TriggerBark();
        }
        public void TriggerBark()
        {
            if (barkGraph == null) return;

            DialogueData data = new DialogueData();

            data.isDialogue = false;

            data.moveCamera = false;
            data.zoomCamera = false;

            data.speakers = new Dictionary<string, SpeakerData>
            {
                { npcID, new SpeakerData { transform = this.transform, textColor = npcTextColor } }
            };

            DialogueManager.Instance.StartDialogue(barkGraph, data);
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
        
        public DialogueData BuildDialogueData(GameObject interactor)
        {
            DialogueData data = new DialogueData();
            data.targetPosition = transform;
            data.cameraOffset = cameraOffset;
            data.zoom = zoom;
            data.smoothTime = smoothTime;
            data.moveCamera = cameraMove;
            data.zoomCamera = zoomCamera;
            data.isDialogue = true;
            data.textColor = npcTextColor;

            data.speakers = new Dictionary<string, SpeakerData>();
            if (interactor != null)
                data.speakers["Conan"] = new SpeakerData { transform = interactor.transform, textColor = Color.white };
            data.speakers[npcID] = new SpeakerData { transform = transform, textColor = npcTextColor };

            return data;
        }
    }
}
