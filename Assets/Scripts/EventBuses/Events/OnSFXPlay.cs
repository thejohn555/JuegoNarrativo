using EventBuses;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.EventBuses
{
    public struct OnSFXPlay : IEvent
    {
        private AudioClip audio;
        public OnSFXPlay(AudioClip audio)
        {
            this.audio = audio;
        }
    }
}