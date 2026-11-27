using Assets.Scripts.Camera;
using System;
using System.Collections.Generic;
using Assets.Scripts.EventBuses;
using Blueprints_arquitectura_Dani_.Runtime;
using EventBuses;
using UnityEngine;
using Assets.Scripts.EventBuses.Events;

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
        [SerializeField] private Dictionary<BarkStateSO, RuntimeDialogueGraph> alternativeBarkStates;
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

        public string currentStateId = ""; //Lo tengo que poner privado
        private GameObject lastInteractor;
        

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus.Subscribe<OnNPCStateChangedEvent>(HandleStateChange);
            EventBus.Subscribe<OnDialogueEndedEvent>(HandleDialogueEnded);
            EventBus.Subscribe<OnBarkStateChangedEvent>(HandleBarkStateChange);
            BlueprintsEventManager.Instance.OnStartBark += Handelbark;
            
        }
        
        private void Handelbark(GameObject obj)
        {
            if (obj != this.gameObject) return;
            TriggerBark();
        }
        private void HandleDialogueEnded(OnDialogueEndedEvent ev)
        {
            if (ev.NpcID != npcID) return;
            BlueprintMediator.Instance.GetLastNpcState(this.currentStateId);
            BlueprintsEventManager.Instance.Raise(NodeTypes.OnDialogueEnd,this.gameObject);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus.UnSubscribe<OnNPCStateChangedEvent>(HandleStateChange);
            EventBus.UnSubscribe<OnDialogueEndedEvent>(HandleDialogueEnded);
            EventBus.UnSubscribe<OnBarkStateChangedEvent>(HandleBarkStateChange);
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
                    currentStateId= state.StateID;
                    currentGraph = state.DialogueGraph;
                    Debug.LogError($"[{npcID}] Dialogue graph changed to state: {ev.NewStateID}");
                    return;
                }
            }

            Debug.Log($"[{npcID}] State not found: {ev.NewStateID}");
        }

        private void HandleBarkStateChange(OnBarkStateChangedEvent ev)
        {
            if (ev.TargetNpc != this.gameObject || ev.NewBarkGraph == null) return;

            if (alternativeBarkStates != null && alternativeBarkStates.TryGetValue(ev.NewBarkGraph, out RuntimeDialogueGraph newGraph))
            {
                barkGraph = newGraph;
            }
            else
            {
                Debug.LogWarning($"[{npcID}] No BarkGraph for '{ev.NewBarkGraph.name}'.");
            }
        }

        public override void Interact(GameObject interactor)
        {
            lastInteractor = interactor;
            DialogueData data = BuildDialogueData();

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

            EventBus.Publish(new OnBarkEvent(barkGraph, transform, npcID, npcTextColor));
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
        
        public DialogueData BuildDialogueData()
        {
            GameObject resolvedInteractor = lastInteractor != null ? lastInteractor : GameObject.FindGameObjectWithTag("Player");

            DialogueData data = new DialogueData();
            data.npcID = npcID;
            data.targetPosition = transform;
            data.cameraOffset = cameraOffset;
            data.zoom = zoom;
            data.smoothTime = smoothTime;
            data.moveCamera = cameraMove;
            data.zoomCamera = zoomCamera;
            data.isDialogue = true;
            data.textColor = npcTextColor;

            data.speakers = new Dictionary<string, SpeakerData>();
            if (resolvedInteractor != null)
                data.speakers["Conan"] = new SpeakerData { transform = resolvedInteractor.transform, textColor = Color.white };
            data.speakers[npcID] = new SpeakerData { transform = transform, textColor = npcTextColor };

            return data;
        }
    }
}
