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
        public OnSFXPlay(SFXID audio)
        {
            this.sfxClip = audio;
            audioEvent = AudioEventsID.NULL;
        }

        public OnSFXPlay(AudioEventsID audioEvent)
        {
            this.audioEvent = audioEvent;
            sfxClip = SFXID.NULL;
        }
    }
}