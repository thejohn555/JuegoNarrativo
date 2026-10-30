using Assets.Scripts.EventBuses;
using EventBuses;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.FridayCode
{
    internal class ObjectToConversation : GamblingTable
    {
        [SerializeField] private RuntimeDialogueGraph graph;
        
        Camera.DialogueData dialogueData;
        Camera.SpeakerData kidnapperData;
        Camera.SpeakerData conanData;

        private void Start()
        {
            conanData.transform = playerMovementSystem.gameObject.transform;
            conanData.textColor = Color.white;
            kidnapperData.transform = this.gameObject.transform;
            kidnapperData.textColor = Color.white;
            dialogueData.speakers.Add("Kidnapper", kidnapperData);
            dialogueData.speakers.Add("Conan", conanData);

            

        }
        protected override void RightClickAction(string itemName)
        {
            if (isOver)
            {
                this.itemName = itemName;
                StopAllCoroutines();
                playerMovementSystem.SetNewDestination(gameObject.transform.position, 1.5f);
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