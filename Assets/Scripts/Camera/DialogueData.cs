using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public struct DialogueData
    {
        public Dictionary<string, Transform> speakers;
        public Transform targetPosition;
        public Vector3 cameraOffset;
        public float zoom;
        public float smoothTime;
        public bool moveCamera;
        public bool zoomCamera;
        public bool isDialogue;
        public Color textColor;

        public DialogueData(
        Dictionary<string, Transform> speakers,
        Transform targetPosition,
        Vector3 cameraOffset,
        float zoom = 5f,
        float smoothTime = 0.2f,
        bool moveCamera = true,
        bool zoomCamera = true,
        bool isDialogue = true,
        Color textColor = default(Color) )
        {
            this.speakers = speakers;
            this.targetPosition = targetPosition;
            this.cameraOffset = cameraOffset;
            this.zoom = zoom;
            this.smoothTime = smoothTime;
            this.moveCamera = moveCamera;
            this.zoomCamera = zoomCamera;
            this.isDialogue = isDialogue;
            this.textColor = textColor == default ? Color.white : textColor;
        }
    }
}