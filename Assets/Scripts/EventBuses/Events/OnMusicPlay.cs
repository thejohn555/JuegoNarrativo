using EventBuses;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.EventBuses.Events
{
    public struct OnMusicPlay : IEvent
    {
        private AudioClip audioClip;

        public OnMusicPlay(AudioClip audioClip)
        {
            this.audioClip = audioClip;
        }
    }
}