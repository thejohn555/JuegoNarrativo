using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public struct DialogueCameraData
    {
        public Transform targetPosition;
        public Vector3 cameraOffset;
        public Vector2 dialogueTextOffset;
        public float zoom;
        public float smoothTime;
    }
}