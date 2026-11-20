using EventBuses;
using System.Collections;
using UnityEngine;
using Audio.Enums;

namespace Assets.Scripts.EventBuses
{
    public struct OnSFXPlay : IEvent
    {
        public AudioEventsID audioEvent;
        public SFXID sfxClip;
        public AudioSource audioSource;

        public OnSFXPlay(SFXID audio, AudioSource audioSource)
        {
            this.sfxClip = audio;
            audioEvent = AudioEventsID.NULL;
            this.audioSource = audioSource;  
        }

        public OnSFXPlay(AudioEventsID audioEvent, AudioSource audioSource)
        {
            this.audioEvent = audioEvent;
            sfxClip = SFXID.NULL;
            this.audioSource = audioSource;
        }
    }
}