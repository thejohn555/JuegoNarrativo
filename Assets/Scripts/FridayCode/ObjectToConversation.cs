using Assets.Scripts.Camera;
using Assets.Scripts.EventBuses;
using EventBuses;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Assets.Scripts.FridayCode
{
    internal class ObjectToConversation : GamblingTable
    {
        [SerializeField] private RuntimeDialogueGraph graph;
        [Header("Dialogue Settings")]
        [SerializeField] private float smoothTime;
        [SerializeField] private bool cameraMove;
        [SerializeField] private bool zoomCamera;
        [SerializeField] private float zoom;
        [SerializeField] private Color npcTextColor;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private string npcID;
 

        Camera.DialogueData dialogueData;
        Camera.SpeakerData kidnapperData;
        Camera.SpeakerData conanData;

        private void Start()
        {
            //conanData.transform = playerMovementSystem.gameObject.transform;
            ////conanData.textColor = Color.white;
            //kidnapperData.transform = this.gameObject.transform;
            ////kidnapperData.textColor = Color.white;
            //dialogueData.speakers.Add("Kidnapper", kidnapperData);
            //dialogueData.speakers.Add("Conan", conanData);
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
               { "Conan", new SpeakerData { transform = playerMovementSystem.transform, textColor = Color.white } },
               { npcID, new SpeakerData { transform = this.transform, textColor = npcTextColor } }
            };




        }
        protected override void RightClickAction(string itemName)
        {
            if (isOver)
            {
                this.itemName = itemName;
                StopAllCoroutines();
                playerMovementSystem.SetNewDestination(gameObject.transform.position, interactionDistance);
                StartCoroutine(RandomShit(DialogueWrapperMethod));
            }
            else
            {
                eventBus.Publish(onNotOver);
            }


        }
        private void DialogueWrapperMethod()
        {
            DialogueManager.Instance.StartDialogue(graph, dialogueData);
        }

        


    }
}