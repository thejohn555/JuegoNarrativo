using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public struct DialogueData
    {
        public Transform targetPosition;
        public Vector3 cameraOffset;
        public Vector3 dialogueTextOffset;
        public float zoom;
        public float smoothTime;
        public bool moveCamera;
        public bool zoomCamera;
        public bool isDialogue;
        public Color textColor;

        public DialogueData(
        Transform targetPosition,
        Vector3 cameraOffset,
        Vector3 dialogueTextOffset,
        float zoom = 5f,
        float smoothTime = 0.2f,
        bool moveCamera = true,
        bool zoomCamera = true,
        bool isDialogue = true,
        Color textColor = default(Color) )
        {
            this.targetPosition = targetPosition;
            this.cameraOffset = cameraOffset;
            this.dialogueTextOffset = dialogueTextOffset;
            this.zoom = zoom;
            this.smoothTime = smoothTime;
            this.moveCamera = moveCamera;
            this.zoomCamera = zoomCamera;
            this.isDialogue = true;
            this.textColor = textColor;
        }
    }
}