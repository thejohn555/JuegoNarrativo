using Assets.Scripts.Camera;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.EventBuses
{
    public struct OnDialogueStartedEvent : IEvent { }
    public struct OnDialogueEndedEvent : IEvent { }
    public struct OnSkipDialogueEvent : IEvent { }
    public struct OnNodeDisplayedEvent : IEvent
    {
        public RuntimeDialogueNode Node { get; private set; }

        public OnNodeDisplayedEvent(RuntimeDialogueNode node)
        {
            Node = node;
        }
    }
    public struct OnDialogueCameraDataEvent : IEvent 
    { 
        public DialogueCameraData DialogueCameraData { get; private set; }

        public OnDialogueCameraDataEvent(DialogueCameraData data)
        {
            DialogueCameraData = data;
        }
    }
}