using EventBuses;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.EventBuses.Events
{
    public struct OnBarkEvent : IEvent
    {
        public RuntimeDialogueGraph barkGraph { get; private set; }
        public Transform speakerTransform { get; private set; }
        public string speakerName { get; private set; }
        public Color textColor { get; private set; }

        public OnBarkEvent(RuntimeDialogueGraph barkGraph, Transform speakerTransform, string speakerName, Color textColor)
        {
            this.barkGraph = barkGraph;
            this.speakerTransform = speakerTransform;
            this.speakerName = speakerName;
            this.textColor = textColor;
        }
    }
}