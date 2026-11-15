using EventBuses;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.EventBuses.Events
{
    public struct OnMusicPlay : IEvent
    {
        private AudioClip audioClip;
        private AudioBank audioBank;

        public OnMusicPlay(AudioClip audioClip)
        {
            this.audioClip = audioClip;
            audioBank = null;
        }

        public OnMusicPlay(AudioBank audioBank)
        {
            this.audioBank = audioBank;
            audioClip = null;
        }
    }
}